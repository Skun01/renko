using System;
using project_z_backend.Entities;
using project_z_backend.Share;

namespace project_z_backend.Interfaces.Services;

public interface ITokenService
{
    string CreateAccessToken(User user);
    string CreateEmailConfirmationToken(Guid userId);
    Result<Guid> GetUserIdFromEmailVerifyToken(string token);
    public string GenerateRefreshToken();
    public void SetRefreshTokenCookie(HttpContext httpContext, string refreshToken);
}


