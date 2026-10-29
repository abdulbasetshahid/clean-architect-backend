using EShop.Application.Models.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EShop.Api.Authentication;

public static class CustomerAuthenticationExtensions
{
    /// <summary>
    /// Adds the Firebase customer scheme next to the admin JWT scheme without changing the default scheme.
    /// </summary>
    public static IServiceCollection AddCustomerAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(
                CustomerAuthenticationDefaults.AuthenticationScheme, _ => { });

        services.AddAuthorization(options =>
        {
            // [Authorize] without a policy (admin endpoints) must only accept the admin JWT scheme.
            options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(CustomerAuthenticationDefaults.CustomerOnlyPolicy, policy => policy
                .AddAuthenticationSchemes(CustomerAuthenticationDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireClaim(CustomerAuthenticationDefaults.FirebaseUidClaimType));
        });

        return services;
    }
}
