using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.ChangeTaskBlockTimeline;

public class ChangeTaskBlockTimelineCommandValidator : AbstractValidator<ChangeTaskBlockTimelineCommand>
{
    public ChangeTaskBlockTimelineCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.CourseId)
            .MustAsync((id, token) => context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курса не существует");
        RuleFor(x => x.TaskBlockId)
            .MustAsync((cmd, id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.CourseId, token))
            .WithMessage("Блока задач не существует или не принадлежит заданному курсу");
        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("Время начала не должно быть позже времени конца");
        RuleFor(x => x.TimeAllowed)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Время выполнения не должно быть нулевым");
    }
}
