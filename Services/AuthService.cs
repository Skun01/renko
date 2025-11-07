using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    private readonly ILogger<AuthService> _logger;
    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userRepo = userRepository;
        _roleRepo = roleRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<User>> GetCurrentUserLoginAsync(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null)
            return Result.Failure<User>(Error.BadRequest("Token is invalid"));

        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Result.Failure<User>(Error.BadRequest("User id in token is invalid"));

        var userResult = await _userRepo.GetByIdAsync(userId);
        if(userResult.IsFalure)
            return Result.Failure<User>(Error.NotFound("User not found"));

        return Result.Success(userResult.Value);
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
        if (existingUser.IsSuccess)
            return Result.Failure(Error.Conflict("User email already exist"));

        // Create user:
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        User newUser = request.ToEntity(passwordHash);

        // get default user role
        var userRoleResult = await _roleRepo.GetByNameAsync(RoleConstants.UserRoleName);
        if (userRoleResult.IsFalure)
            return Result.Failure(Error.InternalError("Default 'user' Role does not found"));

        var userRole = userRoleResult.Value;
        newUser.Roles.Add(userRole!);

        return await _userRepo.AddAsync(newUser);
    }

}
