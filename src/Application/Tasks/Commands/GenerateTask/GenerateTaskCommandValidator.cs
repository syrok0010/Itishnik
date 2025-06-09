using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Tasks.Commands.GenerateTask;

public class GenerateTaskCommandValidator : AbstractValidator<GenerateTaskCommand>
{
    public GenerateTaskCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Topic)
            .NotEmpty()
            .WithMessage("Тема не задана");
        RuleFor(x => x.Difficulty)
            .NotEmpty()
            .WithMessage("Сложность не задана");
        RuleFor(x => x.TaskType)
            .NotEmpty()
            .WithMessage("Тип задачи не задан");
    }
}
