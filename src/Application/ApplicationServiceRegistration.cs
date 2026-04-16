using EShop.Application.Behaviors;
using EShop.Application.Features.Categories.Commands.CreateCategory;
using EShop.Application.Profiles;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper((cfg) => cfg.AddProfile(new MappingProfiles()));

            services.AddValidatorsFromAssemblyContaining<CreateCategoryCommandValidator>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddMediatR((cfg) => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            return services;
        }
    }
}
