using Microsoft.OpenApi.Models;

namespace Data_Center.Configuration;

public static class SwaggerConfiguration
{
    public static void ConfigureSwaggerServices(this WebApplicationBuilder builder)
    {
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
    }
}