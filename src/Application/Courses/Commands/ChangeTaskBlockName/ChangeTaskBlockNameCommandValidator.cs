using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.ChangeTaskBlockName;

public class ChangeTaskBlockNameCommandValidator : AbstractValidator<ChangeTaskBlockNameCommand>
{
    private readonly IApplicationDbContext _context;
    
    public ChangeTaskBlockNameCommandValidator(IApplicationDbContext context)
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
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255)
            .WithMessage("Длина названия не должна превышать 255 символов");
    }
    
    private async Task<bool> CheckRelation(Guid courseId, Guid blockId, CancellationToken cancellationToken)
    {
        var block = await _context.TaskBlocks.FirstAsync(x => x.Id == blockId, cancellationToken);
        return block.CourseId == courseId;
    }
}
