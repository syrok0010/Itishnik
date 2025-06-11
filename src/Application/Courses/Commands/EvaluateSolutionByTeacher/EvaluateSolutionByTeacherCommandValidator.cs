using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.EvaluateSolutionByTeacher;

public class EvaluateSolutionByTeacherCommandValidator : AbstractValidator<EvaluateSolutionByTeacherCommand>
{
    public EvaluateSolutionByTeacherCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.CourseId)
            .MustAsync((id, token) => context.Courses.AnyAsync(e => e.Id == id, token))
            .WithMessage("Курс не существует");
        RuleFor(x => x.TaskBlockId)
            .MustAsync((id, token) => context.TaskBlocks.AnyAsync(e => e.Id == id, token))
            .WithMessage("Работа не существует");
        RuleFor(x => x.TaskId)
            .MustAsync((id, token) => context.Tasks.AnyAsync(e => e.Id == id, token))
            .WithMessage("Задание не существует");
        RuleFor(x => x.SolutionId)
            .MustAsync((id, token) => context.Solutions.AnyAsync(e => e.Id == id, token))
            .WithMessage("Решение не существует");
        RuleFor(x => x)
            .MustAsync((x, token) => CheckScore(context, x, token))
            .WithMessage("Оценка должна быть от нуля до веса задачи");
    }

    private async Task<bool> CheckScore(IApplicationDbContext context, EvaluateSolutionByTeacherCommand command, CancellationToken token)
    {
        var taskBlock = await context.TaskBlocks
            .Include(e => e.TasksEntries)
            .FirstAsync(t => t.Id == command.TaskBlockId, token);
        return command.Grade <= taskBlock.TasksEntries.First(t => t.TaskId == command.TaskId).Weight;
    }
}
