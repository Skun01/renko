using System;
using project_z_backend.Interfaces.Services;
using project_z_backend.Services;

namespace project_z_backend.Extensions;

public static class EndpointServiceExtensions
{
    public static IServiceCollection AddEndpointServicesInjection(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        
        return services;
    }
}
