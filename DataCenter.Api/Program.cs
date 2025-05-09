using System.Text;
using Data_Center.Configuration;
using Data_Center.Configuration.Database;
using Data_Center.Configuration.DI;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Serilog;
using DataCenter.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DataCenter API", Version = "v1" });

    // Tell Swagger how to interpret IFormFile
    c.MapType<IFormFile>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DataCenter")));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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

        options.RequireHttpsMetadata = false; // only for local testing
    });

builder.Services.AddAuthorization();

// Replace default logging with Serilog and Read Serilog config from appsettings.json
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    });
}

// // Run migrations each time the app runs (Only in development)
// if (app.Environment.IsDevelopment())
// {
//     // Only in dev
//     using var scope = app.Services.CreateScope();
//     var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
//     db.Database.Migrate();
//     
//     var authDb = scope.ServiceProvider.GetRequiredService<AuthDatabaseContext>();
//     authDb.Database.Migrate();
// }

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Inject Serilog via Dependency Injection
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started with Serilog!");

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>(); // Register the exception middleware
app.MapHub<UploadProgressHub>("/uploadProgressHub");

// Add Hangfire Dashboard to monitor jobs
app.UseHangfireDashboard();

app.Run();
