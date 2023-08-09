using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Reflection;
using System.Text;
using userMS.Application.DTOs;
using userMS.Application.Filters;
using userMS.Application.Repositories;
using userMS.Application.Services;
using userMS.Application.Validators;
using userMS.Domain.Entities;
using userMS.Infrastructure.Helpers;
using userMS.Infrastructure.Services;
using userMS.Persistence.Data;
using userMS.Persistence.Repositories;
using userMS.Persistence.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// added global exception filter and validation tool
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
})
    .ConfigureApiBehaviorOptions(options =>
{
    // configured option to handle the behaviour of invalid model state - failed validation
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
        .ToDictionary(s => s.Key, s => s.Value.Errors
        .Select(e => e.ErrorMessage).First());

        return new BadRequestObjectResult(new
        {
            ValidationFailures = errors
        });
    };
})
    .AddFluentValidation(config =>
    {
        config.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // no need to register validator for each class
        config.ImplicitlyValidateChildProperties = true;    // necessary to validate lists of objects
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    In = ParameterLocation.Header,
    Description = "Insert JWT Token",
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    BearerFormat = "JWT",
    Scheme = "bearer"
}));

builder.Services.AddSwaggerGen(options =>
options.AddSecurityRequirement(
    new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{ }
        }
    }));



// adding database configuration (in order to use option pattern)
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("MyDb")
    );

// adding jwt configuration (in order to use option pattern)
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtConfig")
    );

// adding email settings configuration (in order to use option pattern)
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings")
    );


// adding email content configuration (in order to use option pattern)
builder.Services.Configure<EmailContent>(
    builder.Configuration.GetSection("EmailContent")
    );


//
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
//

// registering the repositories
// adding data repositories to the dependency injection container
builder.Services.AddScoped<IRepository<User, Guid>, Repository<User, Guid>>();


// registering services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// auto-mapper DI
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// fluent validator DI for UserDto validation
builder.Services.AddTransient<IValidator<UserDto>, UserDtoValidator>();
builder.Services.AddTransient<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();

// DI for validator interceptor
builder.Services.AddTransient<IValidatorInterceptor, ValidatorInterceptor>();


// registering services for authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Secret"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
    });
}

app.UseHttpsRedirection();

// authentication middleware will examine the incoming request for tokens
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
