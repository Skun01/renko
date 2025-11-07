using System;
using Microsoft.AspNetCore.Mvc;
using project_z_backend.DTOs.Auth;
using project_z_backend.Filters;
using project_z_backend.Interfaces.Services;
using project_z_backend.Mapping;

namespace project_z_backend.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder app, string routePrefix)
    {
        var group = app.MapGroup(routePrefix)
            .WithTags("Auth");

        group.MapPost("/register", async (
            IAuthService authService,
            [FromBody] RegisterRequest request
        ) =>
        {
            var result = await authService.RegisterAsync(request);
            return result.ToApiResponse("Create user success");
        }).AddEndpointFilter<ValidationFilter<RegisterRequest>>();

        group.MapPost("/login", async (
            IAuthService authService,
            [FromBody] LoginRequest request
        ) =>
        {
            var result = await authService.LoginAsync(request);
            return result.ToApiResponse("Login success");
        });
        return group;
    }
}
