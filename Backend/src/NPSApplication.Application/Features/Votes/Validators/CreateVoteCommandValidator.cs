using NPSApplication.Application.Features.Votes.Commands;
using FluentValidation;

namespace NPSApplication.Application.Features.Votes.Validators;

public class CreateVoteCommandValidator : AbstractValidator<CreateVoteCommand>
{
    public CreateVoteCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId inválido");

        RuleFor(x => x.Score)
            .InclusiveBetween(0, 10)
            .WithMessage("El score debe estar entre 0 y 10");
    }
}