using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Tasks.Commands.GenerateTask;

public class GenerateTaskCommandValidator : AbstractValidator<GenerateTaskCommand>
{
    public GenerateTaskCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.TaskId)
            .MustAsync((id, token) => context.Tasks.AnyAsync(t => t.Id == id, token))
            .WithMessage("Задача не существует");
    }
}
