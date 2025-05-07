using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.ChangeTaskBlockDescription;

public class ChangeTaskBlockDescriptionCommandValidator : AbstractValidator<ChangeTaskBlockDescriptionCommand>
{
    public ChangeTaskBlockDescriptionCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.CourseId)
            .MustAsync((id, token) => context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курса не существует");
        RuleFor(x => x.TaskBlockId)
            .MustAsync((cmd, id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.CourseId, token))
            .WithMessage("Блока задач не существует или не принадлежит заданному курсу");
    }
}
