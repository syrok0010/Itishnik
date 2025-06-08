using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.PublishTaskBlock;

public class PublishTaskBlockCommandValidator : AbstractValidator<PublishTaskBlockCommand>
{
    public PublishTaskBlockCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.CourseId)
            .MustAsync((id, token) => context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курса не существует");
        RuleFor(x => x.TaskBlockId)
            .Cascade(CascadeMode.Stop)
            .MustAsync((cmd, id, token) =>
                context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.CourseId && !tb.IsPublic, token))
            .WithMessage("Блок задач не существует, не принадлежит заданному курсу или уже опубликован")
            .MustAsync((id, token) => CheckTasksAndWeights(context, id, token))
            .WithMessage("Блок задач пуст или сумма весов не равна 10")
            .MustAsync((id, token) => CheckStartTime(context, id, token))
            .WithMessage("Время начала работы должно быть больше текущего времени");
    }

    private async Task<bool> CheckStartTime(IApplicationDbContext context, Guid taskBlockId, CancellationToken token)
    {
        return await context.TaskBlocks
            .Where(x => x.Id == taskBlockId)
            .Select(x => x.StartTime)
            .FirstAsync(token) > TimeProvider.System.GetLocalNow();
    }

    private Task<bool> CheckTasksAndWeights(IApplicationDbContext context, Guid taskBlockId, CancellationToken cancellationToken)
    {
        return context.TaskBlocks
            .Where(tb => tb.Id == taskBlockId)
            .Select(taskBlock => taskBlock.TasksEntries.Any() && taskBlock.TasksEntries.Sum(e => e.Weight) == 10)
            .FirstAsync(cancellationToken);
    }
}
