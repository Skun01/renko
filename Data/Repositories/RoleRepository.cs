using Microsoft.EntityFrameworkCore;
using project_z_backend.Entities;
using project_z_backend.Interfaces.Repositories;
using project_z_backend.Share;

namespace project_z_backend.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ProjectZContext _context;
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(ProjectZContext context, ILogger<RoleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result> AddAsync(Role entity)
        {
            try
            {
                _context.Roles.Add(entity);
                await _context.SaveChangesAsync();
                return Result.Success();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error adding Role.");
                return Result.Failure(Error.Conflict("Cannot add role, name might already exist."));
            }
        }

        public async Task<Result> UpdateAsync(Role entity)
        {
            try
            {
                _context.Roles.Update(entity);
                await _context.SaveChangesAsync();
                return Result.Success();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error updating Role.");
                return Result.Failure(Error.Conflict("Cannot update role, name might already exist."));
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            try
            {
                var affectedRows = await _context.Roles
                    .Where(r => r.Id == id)
                    .ExecuteDeleteAsync();

                if (affectedRows == 0)
                {
                    return Result.Failure(Error.NotFound("Role not found."));
                }
                
                return Result.Success();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error deleting Role.");
                return Result.Failure(Error.Conflict("Cannot delete this role, it might be in use."));
            }
        }

        public async Task<Result<Role>> GetByIdAsync(Guid id)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == id);
                if (role == null)
                {
                    return Result.Failure<Role>(Error.NotFound("Role not found."));
                }
                return Result.Success(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error getting Role by ID.");
                return Result.Failure<Role>(Error.Conflict("Could not retrieve data from database."));
            }
        }

        public async Task<Result<Role>> GetByNameAsync(string name)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == name);
                if (role == null)
                {
                    return Result.Failure<Role>(Error.NotFound("Role not found by name."));
                }
                return Result.Success(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error getting Role by Name.");
                return Result.Failure<Role>(Error.InternalError("Could not retrieve data from database."));
            }
        }

        public async Task<Result<IEnumerable<Role>>> GetAllAsync()
        {
            try
            {
                var roles = await _context.Roles
                    .AsNoTracking()
                    .ToListAsync();
                return Result.Success<IEnumerable<Role>>(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error getting all Roles.");
                return Result.Failure<IEnumerable<Role>>(Error.InternalError("Could not retrieve data from database."));
            }
        }
    }
}