using System;
using project_z_backend.Entities;
using project_z_backend.Share;

namespace project_z_backend.Interfaces.Repositories;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Result<Role>> GetByNameAsync(string name);
}
