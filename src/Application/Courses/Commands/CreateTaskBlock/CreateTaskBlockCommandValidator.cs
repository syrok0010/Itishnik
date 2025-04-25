using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.CreateTaskBlock;

public class CreateTaskBlockCommandValidator : AbstractValidator<CreateTaskBlockCommand>
{
    private readonly IApplicationDbContext _context;
    
    public CreateTaskBlockCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255)
            .WithMessage("Недопустимое имя блока");
        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("Время начала не должно быть позже времени конца");
        RuleFor(x => x.TimeAllowed)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Время выполнения не должно быть нулевым");
        RuleFor(x => x.TaskIds)
            .MustAsync(AllTaskIdsExist)
            .WithMessage("Одна или несколько задач не существует");

    }

    private async Task<bool> AllTaskIdsExist(IList<Guid> taskIds, CancellationToken cancellationToken)
    {
        var existingTasksCount = await _context.Tasks
            .Where(t => taskIds.Contains(t.Id))
            .CountAsync(cancellationToken);
        
        return existingTasksCount == taskIds.Count;
    }
}
