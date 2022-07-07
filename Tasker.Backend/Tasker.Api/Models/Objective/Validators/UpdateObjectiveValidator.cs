using FluentValidation;
using Tasker.Api.Models.Objective.Requests;

namespace Tasker.Api.Models.Objective.Validators;

public class UpdateObjectiveValidator : AbstractValidator<UpdateObjectiveRequest>
{
    public UpdateObjectiveValidator()
    {
        RuleFor(x => x.Id)
           .NotNull().WithMessage("Objective id cannot be null")
           .Length(36).WithMessage("Objective id length should be 36 symbols");

        RuleFor(x => x.Name)
            .NotNull().WithMessage("Name cannot be null")
            .NotEmpty().WithMessage("Name cannot be empty")
            .MinimumLength(3).WithMessage("Minimum name length must be from 3 symbols")
            .MaximumLength(35).WithMessage("Maximum name length must be less than 35 symbols");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Description cannot be null")
            .NotEmpty().WithMessage("Description cannot be empty")
            .MinimumLength(5).WithMessage("Minimum description length must be from 5 symbols")
            .MaximumLength(150).WithMessage("Maximum description length must be less that 150 symbols");

        RuleFor(x => x.StartAt)
            .NotNull().WithMessage("Start date cannot be null")
            .Must(ValidateStartAt).WithMessage("Start date must be greater than the current one");

        RuleFor(x => x.PeriodInMinutes)
            .NotNull().WithMessage("Period in minutes cannot be null")
            .GreaterThan(5).WithMessage("Period in minutes cannot be less than 5 minutes")
            .LessThan(10080).WithMessage("Period in minutes cannot be greater than 10080 minutes");

        RuleFor(x => x.FreeApiId)
            .NotNull().WithMessage("Api id cannot be null")
            .Length(36).WithMessage("Api id length should be 36 symbols");
    }

    private bool ValidateStartAt(DateTime startAt) => startAt.ToUniversalTime() > DateTime.UtcNow;
}
