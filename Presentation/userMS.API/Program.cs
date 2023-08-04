using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;
using userMS.Application.DTOs;
using userMS.Application.Filters;
using userMS.Application.Repositories;
using userMS.Application.Services;
using userMS.Application.Validators;
using userMS.Domain.Entities;
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
    // configured option to handle the behaviour of invalid model state
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
builder.Services.AddSwaggerGen();

// adding database configuration
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("MyDb")
    );

// registering the repositories
// adding data repositories to the dependency injection container
builder.Services.AddScoped<IRepository<User, Guid>, Repository<User, Guid>>();


// registering services
builder.Services.AddScoped<IUserService, UserService>();

// auto-mapper DI
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// fluent validator DI for UserDto validation
builder.Services.AddTransient<IValidator<UserDto>, UserDtoValidator>();

// DI for validator interceptor
builder.Services.AddTransient<IValidatorInterceptor, ValidatorInterceptor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
