using userMS.Data;
using userMS.Models;
using userMS.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// adding database configuration
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("MyDb")
    );

// registering the repositories
// adding data repositories to the dependency injection container
builder.Services.AddScoped<IGenericRepository<User>,GenericRepository<User>>();

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
