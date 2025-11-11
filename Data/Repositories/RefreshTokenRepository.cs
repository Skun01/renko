using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using project_z_backend.Entities;
using project_z_backend.Interfaces.Repositories;
using project_z_backend.Options;
using project_z_backend.Share;

namespace project_z_backend.Data.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ProjectZContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<RefreshTokenRepository> _logger;
    public RefreshTokenRepository(
        ProjectZContext projectZContext,
        IOptions<JwtSettings> jwtSettings,
        ILogger<RefreshTokenRepository> logger)
    {
        _context = projectZContext;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }
    public async Task<Result> AddAsync(Guid userId, string refreshToken)
    {
        try
        {
            var newToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDays),
                UserId = userId
            };

            _context.RefreshTokens.Add(newToken);
            await _context.SaveChangesAsync();
            return Result.Success();
        }catch(Exception ex)
        {
            _logger.LogError($"Error when Add Refresk token to database: {ex.Message}");
            return Result.Failure(Error.InternalError("Error when create and save refresh token"));
        }
    }

    public Task<Result> DeleteAllTokenAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> DeleteByTokenAsync(string refreshToken)
    {
        var affectedRows = await _context.RefreshTokens
            .Where(r => r.Token == refreshToken)
            .ExecuteDeleteAsync();

        if (affectedRows == 0)
            return Result.Failure(Error.NotFound("Refresh token not found in database"));

        return Result.Success();
    }

    public Task<Result<RefreshToken>> GetByTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }
}
