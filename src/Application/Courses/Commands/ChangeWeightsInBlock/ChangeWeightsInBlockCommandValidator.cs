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
            .Cascade(CascadeMode.Stop)
            .MustAsync((cmd, id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.Id, token))
            .WithMessage("Работа не существует или не принадлежит заданному курсу")
            .MustAsync((id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && !tb.IsPublic, token))
            .WithMessage("Нельзя менять баллы в опубликованной работе");
        RuleFor(x => x.Weights)
            .MustAsync((cmd, list, token) => CheckLength(context, cmd, token))
            .WithMessage("Количество весов не совпадает с количеством заданий");
    }

    private async Task<bool> CheckLength(IApplicationDbContext context, ChangeWeightsInBlockCommand command, CancellationToken cancellationToken)
    {
        return await context.TaskBlocks
            .Where(tb => tb.Id == command.BlockId)
            .Select(tb => tb.TasksEntries.Count())
            .FirstAsync(cancellationToken) == command.Weights.Count;
    }
}
