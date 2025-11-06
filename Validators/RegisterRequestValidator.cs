using FluentValidation;
using project_z_backend.DTOs.Auth;

namespace project_z_backend.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Username)
            .NotEmpty().WithMessage("User name must not be empty")
            .MaximumLength(50).WithMessage("user name must not be exceed 50 characters");

        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("User name must not be empty")
            .EmailAddress().WithMessage("Invalid email format");
        
        RuleFor(r => r.Password)
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number");
    }
}
