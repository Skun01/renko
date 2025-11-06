using System;
using project_z_backend.Share;

namespace project_z_backend.Interfaces.Repositories;

public interface IGenericRepository<T>
{
    Task<Result<T>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<T>>> GetAllAsync();
    Task<Result> UpdateAsync(T entity);
    Task<Result> DeleteAsync(Guid id);
    Task<Result> AddAsync(T entity);
}
