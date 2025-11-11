using System.Security.Claims;
using project_z_backend.DTOs.Auth;
using project_z_backend.DTOs.User;
using project_z_backend.Entities;
using project_z_backend.Interfaces;
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
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly ITokenService _tokenService;
    private readonly IEmailSenderService _emailSender;
    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITokenService tokenService,
        IEmailSenderService emailSender,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepo = userRepository;
        _roleRepo = roleRepository;
        _tokenService = tokenService;
        _emailSender = emailSender;
        _refreshTokenRepo = refreshTokenRepository;
    }

    public async Task<Result<UserResponse>> GetCurrentUserLoginAsync(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null)
            return Result.Failure<UserResponse>(Error.BadRequest("Token is invalid"));

        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Result.Failure<UserResponse>(Error.BadRequest("User id in token is invalid"));

        var userResult = await _userRepo.GetByIdAsync(userId);
        if(userResult.IsFalure)
            return Result.Failure<UserResponse>(Error.NotFound("User not found"));

        return Result.Success(userResult.Value!.ToResponse());
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, HttpContext httpContext)
    {
        //check email
        var existingUser = await _userRepo.GetByEmailAsync(request.Email);
        if (existingUser.IsFalure)
            return Result.Failure<LoginResponse>(Error.NotFound("Email or Password is not valid"));

        // check password
        var user = existingUser.Value;
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user!.PasswordHash
        );
        if (!isPasswordValid)
            return Result.Failure<LoginResponse>(Error.NotFound("Email or Password is not valid"));

        // create access token
        string accessToken = _tokenService.CreateAccessToken(user);
        var response = new LoginResponse(accessToken);

        // create refreshToken
        string refreshTokenString = _tokenService.GenerateRefreshToken();
        var addTokenResult = await _refreshTokenRepo.AddAsync(user.Id, refreshTokenString);
        if (addTokenResult.IsFalure)
            return Result.Failure<LoginResponse>(addTokenResult.Error!);

        // save to cookie
        _tokenService.SetRefreshTokenCookie(httpContext, refreshTokenString);
        
        return Result.Success(response);
    }

    public async Task<Result> LogoutAsync(HttpContext httpContext)
    {
        var refreshTokenString = httpContext.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshTokenString))
            return Result.Success();

        await _refreshTokenRepo.DeleteByTokenAsync(refreshTokenString);

        httpContext.Response.Cookies.Delete("refreshToken");
        return Result.Success();
    }

    public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(HttpContext httpContext)
    {
        // get refresh token from cookie
        var refreshTokenString = httpContext.Request.Cookies["refreskToken"];
        if (string.IsNullOrEmpty(refreshTokenString))
            return Result.Failure<RefreshTokenResponse>(Error.Unauthorized("No refresh token found"));

        // get refresh token object from db
        var storedTokenResult = await _refreshTokenRepo.GetByTokenAsync(refreshTokenString);
        if (storedTokenResult.IsFalure)
            return Result.Failure<RefreshTokenResponse>(storedTokenResult.Error!);

        var storedToken = storedTokenResult.Value;
        if (storedToken!.ExpiresAt < DateTime.UtcNow)
        {
            await _refreshTokenRepo.DeleteByTokenAsync(storedToken.Token);
            return Result.Failure<RefreshTokenResponse>(Error.Unauthorized("Refresh token is expired"));
        }

        var user = storedToken.User;
        if (user is null)
            return Result.Failure<RefreshTokenResponse>(Error.InternalError("User not found for token"));

        // remove old token, create new token
        await _refreshTokenRepo.DeleteByTokenAsync(storedToken.Token);
        string newAccessToken = _tokenService.CreateAccessToken(user);
        string newRefreshTokenString = _tokenService.GenerateRefreshToken();
        await _refreshTokenRepo.AddAsync(user.Id, newRefreshTokenString);
        
        // set new refresh token into cookie
        _tokenService.SetRefreshTokenCookie(httpContext, newRefreshTokenString);

        return Result.Success(new RefreshTokenResponse(newAccessToken));
        
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

        var createUserResult = await _userRepo.AddAsync(newUser);
        if (createUserResult.IsFalure)
            return createUserResult;

        // Create and send email confirm token link
        string verifyEmailtoken = _tokenService.CreateEmailConfirmationToken(newUser.Id);
        var emailSendingResult = await _emailSender.SendVerifyEmailAsync(newUser.UserName, newUser.Email, verifyEmailtoken);
        if (emailSendingResult.IsFalure)
            Result.Failure(Error.InternalError("Create user success but cannot send verify email to user"));

        return Result.Success();
    }

    public async Task<Result> VerifyEmailAsync(string token)
    {
        var userIdResult = _tokenService.GetUserIdFromEmailVerifyToken(token);
        if (userIdResult.IsFalure)
            return Result.Failure(userIdResult.Error!);
        return await _userRepo.UpdateVerifyEmailByIdAsync(userIdResult.Value, true);
    }
}
