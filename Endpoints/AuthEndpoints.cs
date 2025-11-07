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

        group.MapPost("/register", async(
            [FromBody] RegisterRequest request,
            IAuthService authService,
            HttpContext context
        ) =>
        {
            var result = await authService.RegisterAsync(request, context, "confirmEmailEndpoint");
            return result.ToApiResponse("Check email to verify your email.");
        }).AddEndpointFilter<ValidationFilter<RegisterRequest>>();

        group.MapPost("/login", async (
            IAuthService authService,
            [FromBody] LoginRequest request
        ) =>
        {
            var result = await authService.LoginAsync(request);
            return result.ToApiResponse("Login success");
        });

        group.MapGet("/me", async (
            IAuthService authService,
            HttpContext context
        ) =>
        {
            var result = await authService.GetCurrentUserLoginAsync(context);
            return result.ToApiResponse("Get current user information sucess");
        }).RequireAuthorization();

        group.MapGet("/confirm-email", async (
            [FromQuery] string token,
            IAuthService authService
        ) =>
        {
            var result = await authService.VerifyEmailAsync(token);
            return result.ToApiResponse("Verify email successful.");
        }).WithName("confirmEmailEndpoint");
        return group;
    }
}
