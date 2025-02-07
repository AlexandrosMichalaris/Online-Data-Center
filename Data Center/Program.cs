using Data_Center.Configuration;
using Data_Center.Configuration.Database;
using Data_Center.Configuration.DI;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure services using the static class method
builder.Services.ConfigureServices();

builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DataCenter")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// Replace default logging with Serilog and Read Serilog config from appsettings.json
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Inject Serilog via Dependency Injection
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started with Serilog!");

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>(); // Register the exception middleware

app.MapControllers();

app.UseRouting();

app.Run();
