using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.ChangeTaskBlockTimeline;

public class ChangeTaskBlockTimelineCommandValidator : AbstractValidator<ChangeTaskBlockTimelineCommand>
{
    private readonly IApplicationDbContext _context;
    
    public ChangeTaskBlockTimelineCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        RuleFor(x => x.CourseId)
            .MustAsync((id, token) => _context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курса не существует");
        RuleFor(x => x.TaskBlockId)
            .MustAsync((id, token) => _context.TaskBlocks.AnyAsync(tb => tb.Id == id, token))
            .WithMessage("Блока задач не существует");
        RuleFor(x => x)
            .MustAsync((command, token) => CheckRelation(command.CourseId, command.TaskBlockId, token))
            .WithMessage("Блок задач не принадлежит курсу");
        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("Время начала не должно быть позже времени конца");
        RuleFor(x => x.TimeAllowed)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Время выполнения не должно быть нулевым");
    }

    private async Task<bool> CheckRelation(Guid courseId, Guid blockId, CancellationToken cancellationToken)
    {
        var block = await _context.TaskBlocks.FirstAsync(x => x.Id == blockId, cancellationToken);
        return block.CourseId == courseId;
    }
}
