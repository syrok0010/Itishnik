using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Students.StartTaskBlock;

public class StartTaskBlockCommandValidator : AbstractValidator<StartTaskBlockCommand>
{
    public StartTaskBlockCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Id)
            .MustAsync((id, token) => context.GradedCourses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курс не существует");
        RuleFor(x => x.BlockId)
            .MustAsync((id, token) => context.GradedTaskBlocks.AnyAsync(b => b.Id == id, token))
            .WithMessage("Работа не существует");
        RuleFor(x => x)
            .MustAsync((command, token) => IsAvailable(context, command, token))
            .WithMessage("Работа еще не начата или уже закончена");
    }

    private async Task<bool> IsAvailable(IApplicationDbContext context, StartTaskBlockCommand command, CancellationToken token)
    {
        var block = await context.GradedTaskBlocks
            .Include(b => b.TaskBlock)
            .FirstAsync(b => b.Id == command.BlockId, token);
        return block.TaskBlock.StartTime <= DateTime.UtcNow && DateTime.UtcNow <= block.TaskBlock.EndTime;
    }
}
