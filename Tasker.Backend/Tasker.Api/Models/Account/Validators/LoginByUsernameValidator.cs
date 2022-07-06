using FluentValidation;
using Tasker.Api.Models.Account.Requests;

namespace Tasker.Api.Models.Account.Validators;

public class LoginByUsernameValidator : AbstractValidator<LoginByUsernameRequest>
{
    public LoginByUsernameValidator()
    {
        RuleFor(x => x.Username)
            .NotNull().WithMessage("Username cannot be null")
            .NotEmpty().WithMessage("Username cannot be empty")
            .MinimumLength(3).WithMessage("Minimum username length must be from 3 symbols")
            .MaximumLength(24).WithMessage("Maximum username length must be less than 24 symbols");

        RuleFor(x => x.Password)
            .NotNull().WithMessage("Password cannot be null")
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(8).WithMessage("Minimum password length must be from 8 symbols")
            .MaximumLength(32).WithMessage("Maximum password length must be less than 32 symbols");
    }
}
