using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.ChangeTaskBlockName;

public class ChangeTaskBlockNameCommandValidator : AbstractValidator<ChangeTaskBlockNameCommand>
{
    public ChangeTaskBlockNameCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255)
            .WithMessage("Недопустимое имя блока");
        RuleFor(x => x.CourseId)
            .MustAsync((id, token) => context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курса не существует");
        RuleFor(x => x.TaskBlockId)
            .MustAsync((cmd, id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.CourseId, token))
            .WithMessage("Блока задач не существует или не принадлежит заданному курсу");
    }
}
