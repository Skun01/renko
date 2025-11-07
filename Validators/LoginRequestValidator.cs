using System;
using FluentValidation;
using project_z_backend.DTOs.Auth;

namespace project_z_backend.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email must not be empty")
            .EmailAddress().WithMessage("Invalid Email Form");

        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password must not be empty");
    }
}
