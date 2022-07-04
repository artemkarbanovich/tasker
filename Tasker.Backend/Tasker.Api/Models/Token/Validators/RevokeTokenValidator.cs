using FluentValidation;
using Tasker.Api.Models.Token.Requests;

namespace Tasker.Api.Models.Token.Validators;

public class RevokeTokenValidator : AbstractValidator<RevokeTokenRequest>
{
    public RevokeTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotNull().WithMessage("Refresh token cannot be null")
            .NotEmpty().WithMessage("Refresh token cannot be empty")
            .Length(44).WithMessage("Refresh token length must be 44");
    }
}
