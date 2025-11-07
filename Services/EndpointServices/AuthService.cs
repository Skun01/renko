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
    private readonly ITokenService _tokenService;
    private readonly LinkGenerator _linkGenerator;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IEmailSenderService _emailSender;
    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITokenService tokenService,
        LinkGenerator linkGenerator,
        IEmailTemplateService emailTemplateService,
        IEmailSenderService emailSender)
    {
        _userRepo = userRepository;
        _roleRepo = roleRepository;
        _tokenService = tokenService;
        _linkGenerator = linkGenerator;
        _emailTemplateService = emailTemplateService;
        _emailSender = emailSender;
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

    public async Task<Result> RegisterAsync(RegisterRequest request, HttpContext context, string targetEndpointName)
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
        string token = _tokenService.CreateEmailConfirmationToken(newUser);
        var callbackUrl = _linkGenerator.GetUriByName(
            context, targetEndpointName, new { token }
        );

        string emailTemplate = _emailTemplateService.GetEmailConfirmationTemplate(
            newUser.UserName,
            callbackUrl!
        );

        await _emailSender.SendEmailAsync(newUser.Email, "Confirm your email", emailTemplate);
        return Result.Success();
    }

    public async Task<Result> VerifyEmailAsync(string token)
    {
        Guid? userId = _tokenService.GetUserIdFromEmailVerifyToken(token);
        if (userId is null)
            return Result.Failure(Error.BadRequest("Verify Email token is not valid"));
        return await _userRepo.UpdateVerifyEmailByIdAsync((Guid)userId, true);
    }
}
