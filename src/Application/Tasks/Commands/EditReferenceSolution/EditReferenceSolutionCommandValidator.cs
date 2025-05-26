using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Tasks.Commands.EditReferenceSolution;

public class EditReferenceSolutionCommandValidator : AbstractValidator<EditReferenceSolutionCommand>
{
    public EditReferenceSolutionCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.TaskId)
            .MustAsync((id, token) => context.Tasks.AnyAsync(t => t.Id == id, token))
            .WithMessage("Задание не существует");
        RuleFor(x => x.Text)
            .NotNull()
            .NotEmpty()
            .WithMessage("Текст пуст");
    }
}
