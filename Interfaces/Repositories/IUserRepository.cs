using System;
using project_z_backend.Entities;
using project_z_backend.Share;

namespace project_z_backend.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<Result<User>> GetByEmailAsync(string email);
}
