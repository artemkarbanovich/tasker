using FluentValidation;
using Tasker.Api.Models.Account.Requests;

namespace Tasker.Api.Models.Account.Validators;

public class LoginByEmailValidator : AbstractValidator<LoginByEmailRequest>
{
    public LoginByEmailValidator()
    {
        RuleFor(x => x.Email)
           .NotNull().WithMessage("Email cannot be null")
           .NotEmpty().WithMessage("Email cannot be empty")
           .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotNull().WithMessage("Password cannot be null")
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(8).WithMessage("Minimum password length must be from 8 symbols")
            .MaximumLength(32).WithMessage("Maximum password length must be less than 32 symbols");
    }
}
