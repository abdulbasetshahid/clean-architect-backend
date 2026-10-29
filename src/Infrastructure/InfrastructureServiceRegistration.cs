using EShop.Application.Contracts;
using EShop.Application.Contracts.Infrastructure;
using EShop.Application.Models.Authentication;
using EShop.Infrastructure.Authentication;
using EShop.Infrastructure.FileExport;
using EShop.Infrastructure.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            //services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.Configure<FirebaseSettings>(configuration.GetSection("Firebase"));
            services.AddSingleton<IFirebaseTokenVerifier, FirebaseTokenVerifier>();

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ICsvExporter, CsvExporter>();

            return services;
        }
    }
}
