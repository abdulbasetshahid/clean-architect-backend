using EShop.Application;
using EShop.Infrastructure;
using EShop.Persistence;

namespace EShop.Api
{
    public static class StartupExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddPersistenceServices(builder.Configuration);

            builder.Services.AddControllers();

            builder.Services.AddCors(options => options.AddPolicy("open", policy => policy.WithOrigins([
                builder.Configuration["ApiUrl"] ?? "https://localhost:5001",
                builder.Configuration["WebUrl"] ?? "https://localhost:3000"
                ]).AllowAnyMethod()
                .SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowCredentials()));

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseCors("open");
            app.UseHttpsRedirection();
            app.MapControllers();

            return app;
        }
    }
}
