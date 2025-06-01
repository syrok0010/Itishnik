using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Students.EditSolution;

public class EditSolutionCommandValidator : AbstractValidator<EditSolutionCommand>
{
    public EditSolutionCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.BlockId)
            .MustAsync((id, token) => context.GradedTaskBlocks.AnyAsync(b => b.Id == id, token))
            .WithMessage("Работа не существует");
        RuleFor(x => x.TaskId)
            .MustAsync((id, token) => context.GradedCourses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Задание не существует");
        RuleFor(x => x.Text)
            .NotNull()
            .NotEmpty()
            .WithMessage("Текст решения пустой");
        RuleFor(x => x)
            .MustAsync((command, token) => CanEdit(context, command, token))
            .WithMessage("Невозможно отправить решение");
    }

    private async Task<bool> CanEdit(IApplicationDbContext context, EditSolutionCommand command, CancellationToken token)
    {
        var block = await context.GradedTaskBlocks
            .Include(b => b.TaskBlock)
            .FirstAsync(b => b.Id == command.BlockId, token);
        if (!block.TaskBlock.IsPublic ||
            block.TaskBlock.StartTime is null ||
            block.TaskBlock.EndTime is null ||
            block.TaskBlock.TimeAllowed is null)
        {
            return false;
        }
        
        var endTime = block.TaskBlock.EndTime.Value;
        var timeAllowed = block.TaskBlock.TimeAllowed.Value;
        var studentStartTime = block.StartTime;


        var nearestEnd = studentStartTime + timeAllowed < endTime 
            ? studentStartTime + timeAllowed 
            : endTime;

        return studentStartTime <= DateTime.Now && DateTime.Now <= nearestEnd;
    }
}
