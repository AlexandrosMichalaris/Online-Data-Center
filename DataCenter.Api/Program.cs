using Data_Center.Configuration;
using Data_Center.Configuration.DI;
using Hangfire;
using QueueMessageManagement.Extentions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure services using the static class method
builder.Services.ConfigureDiServices();

//Config for RabbitMq connection options
builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));

builder.ConfigureDatabaseContextServices();

builder.Services.AddEndpointsApiExplorer();
builder.ConfigureSwaggerServices();

builder.Services.AddControllers();
builder.Services.AddSignalR();

// RabbitMq Manager
builder.Services.AddQueueMessageManagement(builder.Configuration.GetSection("RabbitMq"));

builder.ConfigureHangfireServices();

builder.ConfigureAuthenticationServices();

builder.ConfigureRedisService();

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
