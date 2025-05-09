using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.ChangeWeightsInBlock;

public class ChangeWeightsInBlockCommandValidator : AbstractValidator<ChangeWeightsInBlockCommand>
{
    public ChangeWeightsInBlockCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Id)
            .MustAsync((id, token) => context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курса не существует");
        RuleFor(x => x.BlockId)
            .MustAsync((cmd, id, token) =>
                context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.Id, token))
            .WithMessage("Блока задач не существует или не принадлежит заданному курсу");
        RuleFor(x => x.Weights)
            .MustAsync((cmd, list, token) => CheckLength(context, cmd, token))
            .WithMessage("Количество весов не совпадает с количеством заданий");
    }

    private async Task<bool> CheckLength(IApplicationDbContext context, ChangeWeightsInBlockCommand command, CancellationToken cancellationToken)
    {
        var taskBlock = await context.TaskBlocks.Include(tb => tb.Weights).FirstAsync(tb => tb.Id == command.BlockId, cancellationToken);
        return taskBlock.Weights.Count() == command.Weights.Count();
    }
}
