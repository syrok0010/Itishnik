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
            .MustAsync((cmd, id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.CourseId, token))
            .WithMessage("Блока задач не существует или не принадлежит заданному курсу");
    }
}
