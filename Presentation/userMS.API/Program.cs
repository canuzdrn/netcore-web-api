using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Octokit;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using userMS.Application.DTOs;
using userMS.Application.Filters;
using userMS.Application.Repositories;
using userMS.Application.Services;
using userMS.Application.Validators;
using userMS.Infrastructure.Com;
using userMS.Infrastructure.Services;
using userMS.Persistence.Data;
using userMS.Persistence.Repositories;
using userMS.Persistence.Services;
using ProductHeaderValue = Octokit.ProductHeaderValue;
using User = userMS.Domain.Entities.User;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

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



builder.Services.Configure<AppSettings>(configuration);
var appSettings = configuration.Get<AppSettings>();


// adding database configuration (in order to use option pattern)
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("MyDb")
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
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();

builder.Services.AddHttpClient<IFirebaseAuthService, FirebaseAuthService>();

// auto-mapper DI
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// fluent validator DI for UserDto validation
builder.Services.AddTransient<IValidator<UserDto>, UserDtoValidator>();
builder.Services.AddTransient<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddTransient<IValidator<UsernameOrEmailLoginUserDto>, UsernameOrEmailLoginUserDtoValidator>();
builder.Services.AddTransient<IValidator<PhoneLoginUserDto>, PhoneLoginUserDtoValidator>();

// DI for validator interceptor
builder.Services.AddTransient<IValidatorInterceptor, ValidatorInterceptor>();

// register cache service
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();


// configuring authentication
var firebaseIssuer = $"https://securetoken.google.com/{configuration["FirebaseProjectId"]}";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = firebaseIssuer;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = firebaseIssuer,
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["FirebaseProjectId"],
                        ValidateLifetime = true,
                    };
                });

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = configuration["Google:ClientId"];
        options.ClientSecret = configuration["Google:ClientSecret"];
        options.SaveTokens = true;
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = configuration["Microsoft:ClientId"];
        options.ClientSecret = configuration["Microsoft:ClientSecret"];
        options.SaveTokens = true;
    })
    .AddTwitter(options =>
    {
        options.ConsumerKey = configuration["Twitter:ApiKey"];
        options.ConsumerSecret = configuration["Twitter:ApiKeySecret"];
        options.SaveTokens = true;

        options.Events = new TwitterEvents
        {
            OnCreatingTicket = context =>
            {
                // Extract and add the oauth_token_secret as a claim
                context.Principal.AddIdentity(new ClaimsIdentity(new[]
                {
                    new Claim("oauth_token_secret", context.AccessTokenSecret)
                }));

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Github";
})
    .AddOAuth("Github", options => {
        options.ClientId = configuration["Github:ClientId"];
        options.ClientSecret = configuration["Github:ClientSecret"];
        options.CallbackPath = new PathString("/signin-github");

        options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        options.TokenEndpoint = "https://github.com/login/oauth/access_token";
        options.UserInformationEndpoint = "https://api.github.com/user";
        options.ClaimsIssuer = "GitHub";

        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
        options.ClaimActions.MapJsonKey("urn:github:name", "name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email", ClaimValueTypes.Email);

        options.SaveTokens = true;

        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var client = new GitHubClient(new ProductHeaderValue("tg-core-talent-oauth"));
                client.Credentials = new Credentials(context.AccessToken);
                var user = await client.User.Current();

                var userJson = JsonSerializer.Serialize(user);

                // Parse the JSON string into a JsonElement
                using (JsonDocument doc = JsonDocument.Parse(userJson))
                {
                    context.RunClaimActions(doc.RootElement);
                }
            }
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
