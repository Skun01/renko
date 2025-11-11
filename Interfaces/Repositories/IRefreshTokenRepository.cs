using System;
using project_z_backend.Entities;
using project_z_backend.Share;

namespace project_z_backend.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<Result> AddAsync(Guid userId, string refreshToken);
    Task<Result<RefreshToken>> GetByTokenAsync(string refreshToken);
    Task<Result> DeleteAllTokenAsync(Guid userId);
    Task<Result> DeleteByTokenAsync(string refreshToken);
}
