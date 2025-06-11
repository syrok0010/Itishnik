using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Students.EditSolution;

public class EditSolutionCommandValidator : AbstractValidator<EditSolutionCommand>
{
    public EditSolutionCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.BlockId)
            .MustAsync((id, token) => context.GradedTaskBlocks.AnyAsync(b => b.Id == id, token))
            .WithMessage("Работа не существует");
        RuleFor(x => x.Text)
            .NotNull()
            .NotEmpty()
            .WithMessage("Текст решения пустой");
        RuleFor(x => x.SolutionId)
            .MustAsync((id, token) => context.Solutions.AnyAsync(s => s.Id == id, token))
            .WithMessage("Решение не существует");
        RuleFor(x => x)
            .MustAsync((command, token) => CanEdit(context, command, token))
            .WithMessage("Нельзя отправить решение после дедлайна");
    }

    private async Task<bool> CanEdit(IApplicationDbContext context, EditSolutionCommand command, CancellationToken token)
    {
        var block = await context.GradedTaskBlocks
            .Include(b => b.TaskBlock)
            .FirstAsync(b => b.Id == command.BlockId, token);
        if (!block.TaskBlock.IsPublic || block.TaskBlock.StartTime is null || block.TaskBlock.EndTime is null)
            return false;

        if (block.TaskBlock.TimeAllowed is null)
            return DateTime.UtcNow <= block.TaskBlock.EndTime;
        
        var endTime = block.TaskBlock.EndTime;
        var timeAllowed = block.TaskBlock.TimeAllowed;
        var studentStartTime = block.StartTime;

        var nearestEnd = studentStartTime + timeAllowed < endTime 
            ? studentStartTime + timeAllowed 
            : endTime;

        return studentStartTime <= DateTime.UtcNow && DateTime.UtcNow <= nearestEnd;
    }
}
