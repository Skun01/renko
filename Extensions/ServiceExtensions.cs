using System;
using Microsoft.EntityFrameworkCore;
using project_z_backend.Data;

namespace project_z_backend.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration["ConnectionStrings:Sql"]!;
        services.AddDbContext<ProjectZContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
}
