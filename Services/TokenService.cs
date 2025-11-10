using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using project_z_backend.Entities;
using project_z_backend.Interfaces.Services;
using project_z_backend.Options;
using project_z_backend.Share;

namespace project_z_backend.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly SymmetricSecurityKey _secretKey;
    private readonly (string ClaimName, string ClaimDes) VerifyEmailClam = ("purpose", "email_confirmation");
    public TokenService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
        _secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key)
        );
    }
    public string CreateAccessToken(User user)
    {
        // Get data from appsettings 
        var issuer = _jwtSettings.Issuer;
        var audience = _jwtSettings.Audience;
        var expireHours = _jwtSettings.ExpireHours;
        // Create claim
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

        // add role to token
        foreach(var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        // Create token 
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(expireHours),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                _secretKey,
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string CreateEmailConfirmationToken(Guid userId)
    {
        var credenticals = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256Signature);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(VerifyEmailClam.ClaimName, VerifyEmailClam.ClaimDes)
        };
        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddHours(_jwtSettings.EmailTokenExpireHours),
            claims: claims,
            signingCredentials: credenticals
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public Result<Guid> GetUserIdFromEmailVerifyToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return Result.Failure<Guid>(Error.BadRequest("Token is null"));
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = _secretKey,
            ClockSkew = TimeSpan.Zero
        };
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var purposeClaim = principal.FindFirst(VerifyEmailClam.ClaimName);
            if (purposeClaim is null || purposeClaim.Value != VerifyEmailClam.ClaimDes)
                return Result.Failure<Guid>(Error.BadRequest("Token is invalid"));

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return Result.Failure<Guid>(Error.BadRequest("Token is invalid"));

            return Result.Success(userId);
        }
        catch (SecurityTokenException)
        {
            return Result.Failure<Guid>(Error.BadRequest("Token is invalid or has expired."));
        }
        catch (Exception)
        {
            return Result.Failure<Guid>(Error.InternalError("Something is wrong when validate verify token"));
        }
    }
}
