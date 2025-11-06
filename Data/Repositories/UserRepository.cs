using Microsoft.EntityFrameworkCore;
using project_z_backend.Entities;
using project_z_backend.Interfaces.Repositories;
using project_z_backend.Share;

namespace project_z_backend.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ProjectZContext _context;
    private readonly ILogger<UserRepository> _logger;
    public UserRepository(ProjectZContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> AddAsync(User entity)
    {
        try
        {
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("Error database cannot add User: {message}", ex.Message);
            return Result.Failure(Error.Conflict("User email already Exist"));
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            int affectedRows = await _context.Users
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

            if (affectedRows == 0)
                return Result.Failure(Error.NotFound("User not found"));

            return Result.Success();
        }
        catch(DbUpdateException ex)
        {
            _logger.LogError("Error Database cannot delete user: {message}", ex.Message);
            return Result.Failure(Error.BadRequest("Cannot delete User"));
        }
    }

    public async Task<Result<IEnumerable<User>>> GetAllAsync()
    {
        try
        {
            var users = await _context.Users
            .AsNoTracking()
            .ToListAsync();

            return Result.Success<IEnumerable<User>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error database cannot get all user: {message}", ex.Message);
            return Result.Failure<IEnumerable<User>>(Error.BadRequest("Cannot get users from database"));
        }
    }

    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        try
        {
            var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
                return Result.Failure<User>(Error.NotFound("User not found"));

            return Result.Success(user);
        }catch(Exception ex)
        {
            _logger.LogError("Cannot get user by email: {message}", ex.Message);
            return Result.Failure<User>(Error.BadRequest("Cannot get user by email"));
        }
    }

    public async Task<Result<User>> GetByIdAsync(Guid id)
    {
        try
        {
            var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return Result.Failure<User>(Error.NotFound("User Id not found"));

            return Result.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error database cannot get user from Id {message}", ex.Message);
            return Result.Failure<User>(Error.BadRequest("Cannot get user from id"));
        }
    }

    public async Task<Result> UpdateAsync(User entity)
    {
        try
        {
            _context.Users.Update(entity);
            var affectedRows = await _context.SaveChangesAsync();
            if (affectedRows == 0)
                return Result.Failure(Error.Conflict("Cannot update user"));

            return Result.Success();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("Cannot update User: {message}", ex.Message);
            return Result.Failure(Error.Conflict("Cannot update User"));
        }
    }
}
