using project_z_backend.DTOs.Auth;
using project_z_backend.Entities;
using project_z_backend.Interfaces.Repositories;
using project_z_backend.Interfaces.Services;
using project_z_backend.Mapping;
using project_z_backend.Share;
using project_z_backend.Share.Constants;

namespace project_z_backend.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IRoleRepository _roleRepo;
    private readonly ITokenService _tokenService;
    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, ITokenService tokenService)
    {
        _userRepo = userRepository;
        _roleRepo = roleRepository;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var existingUser = await _userRepo.GetByEmailAsync(request.Email);
        if (existingUser.IsFalure)
            return Result.Failure<LoginResponse>(Error.NotFound("Email or Password is not valid"));

        var user = existingUser.Value;
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user!.PasswordHash
        );

        // if password is not valid
        if (!isPasswordValid)
            return Result.Failure<LoginResponse>(Error.NotFound("Email or Password is not valid"));

        // create login token
        string accessToken = _tokenService.CreateAccessToken(user);
        var response = new LoginResponse(accessToken);

        return Result.Success(response);
    }

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepo.GetByEmailAsync(request.Email);
        if (existingUser.Value is not null)
            return Result.Failure(Error.Conflict("User email already exist"));

        // Create user:
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        User newUser = request.ToEntity(passwordHash);

        // get default user role
        var userRoleResult = await _roleRepo.GetByNameAsync(RoleConstants.UserRoleName);
        if (userRoleResult.IsFalure)
            return Result.Failure(Error.InternalError("Does not found role 'user"));

        var userRole = userRoleResult.Value;
        newUser.Roles.Add(userRole!);

        return await _userRepo.AddAsync(newUser);
    }
}
