using project_z_backend.Data.Repositories;
using project_z_backend.Interfaces.Repositories;

namespace project_z_backend.Extensions;

public static class RepositoryServiceExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        return services;
    }
}
