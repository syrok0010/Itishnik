using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.ChangeTaskBlockTimeline;

public class ChangeTaskBlockTimelineCommandValidator : AbstractValidator<ChangeTaskBlockTimelineCommand>
{
    public ChangeTaskBlockTimelineCommandValidator(IApplicationDbContext context)
    {
        var currentTime = TimeProvider.System.GetLocalNow().UtcDateTime;
        RuleFor(x => x.CourseId)
            .MustAsync((id, token) => context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курса не существует");
        RuleFor(x => x.TaskBlockId)
            .Cascade(CascadeMode.Stop)
            .MustAsync((cmd, id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.CourseId, token))
            .WithMessage("Работа не существует или не принадлежит заданному курсу")
            .MustAsync((cmd, id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && (!tb.IsPublic || cmd.StartTime == tb.StartTime || (currentTime < tb.StartTime && cmd.StartTime >= tb.StartTime)), token))
            .WithMessage("Невозможно сдвинуть начало выполнения опубликованной работы назад")
            .MustAsync((cmd, id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && (!tb.IsPublic || cmd.EndTime >= tb.EndTime), token))
            .WithMessage("Невозможно сдвинуть конец выполнения опубликованной работы назад")
            .MustAsync((cmd, id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && !tb.IsPublic, token))
            .WithMessage("Невозможно изменить время на выполнение опубликованной работы");
        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("Время начала не должно быть позже времени конца");
        RuleFor(x => x)
            .Must(x => x.StartTime + x.TimeAllowed <= x.EndTime)
            .Unless(x => x.TimeAllowed == null)
            .WithMessage("Время на выполнение работы превышает время конца работы");
        RuleFor(x => x.TimeAllowed)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Время выполнения не должно быть нулевым")
            .Unless(x => x.TimeAllowed == null);
    }
}
