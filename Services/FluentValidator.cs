using FluentValidation;
using Wajeb.API.Dtos;

namespace Wajeb.API.Services;

// Validators for CreateUser and UpdateUser DTOs
public class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(5).WithMessage("Username must be at least 5 characters long");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required") 
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email is required");
    }
}

public class UpdateUserValidator : AbstractValidator<ChangePasswordDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Old password is required")
            .MinimumLength(6).WithMessage("Old password must be at least 6 characters long");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters long");
    }
}