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
        RuleFor(x => x.TaskIds)
            .MustAsync(AllTaskIdsExist)
            .WithMessage("Одна или несколько задач не существует");
        RuleFor(x => x)
            .Must(x => x.Weights.Count == x.TaskIds.Count)
            .WithMessage("Количества заданий и весов должны совпадать");

    }

    private async Task<bool> AllTaskIdsExist(IList<Guid> taskIds, CancellationToken cancellationToken)
    {
        var existingTasksCount = await _context.Tasks
            .Where(t => taskIds.Contains(t.Id))
            .CountAsync(cancellationToken);
        
        return existingTasksCount == taskIds.Count;
    }
}
