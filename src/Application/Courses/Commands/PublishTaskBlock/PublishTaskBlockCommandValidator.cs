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
            .MustAsync((cmd, id, token) =>
                context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.CourseId && !tb.IsPublic, token))
            .WithMessage("Блок задач не существует, не принадлежит заданному курсу или опубликован");
        RuleFor(x => x.TaskBlockId)
            .MustAsync((cmd, id, token) => CheckTasksAndWeights(context, cmd.CourseId, id, token))
            .WithMessage("Блок задач пуст или сумма весов не равна 10");
    }

    private async Task<bool> CheckTasksAndWeights(IApplicationDbContext context, Guid courseId, Guid taskBlockId,
        CancellationToken cancellationToken)
    {
        var taskBlock = await context.TaskBlocks
            .Where(tb => tb.Id == taskBlockId && tb.CourseId == courseId)
            .Include(tb => tb.Tasks)
            .FirstAsync(cancellationToken);
        return taskBlock.Tasks.Any(t => t.IsPublic) && taskBlock.Weights.Sum() == 10;
    }
}
