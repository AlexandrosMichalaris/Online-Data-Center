using System.Text;
using Data_Center.Configuration;
using Data_Center.Configuration.Database;
using Data_Center.Configuration.DI;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Serilog;
using AutoMapper;
using DataCenter.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configure services using the static class method
builder.Services.ConfigureServices();

builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DataCenter")));

builder.Services.AddDbContext<AuthDatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DataCenter")));

builder.Services.AddIdentity<ApplicationUserEntity, IdentityRole>()
    .AddEntityFrameworkStores<AuthDatabaseContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DataCenter")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });
builder.Services.AddAuthorization();

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
app.MapHub<UploadProgressHub>("/uploadProgressHub");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Add Hangfire Dashboard to monitor jobs
app.UseHangfireDashboard();

app.Run();
