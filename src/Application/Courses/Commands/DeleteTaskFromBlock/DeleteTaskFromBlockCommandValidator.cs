using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.DeleteTaskFromBlock;

public class DeleteTaskFromBlockCommandValidator : AbstractValidator<DeleteTaskFromBlockCommand>
{
    public DeleteTaskFromBlockCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Id)
            .MustAsync((id, token) => context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курса не существует");
        RuleFor(x => x.BlockId)
            .MustAsync((cmd, id, token) =>
                context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.Id, token))
            .WithMessage("Блока задач не существует или не принадлежит заданному курсу");
        RuleFor(x => x.TaskId)
            .MustAsync((id, token) => context.Tasks.AnyAsync(t => t.Id == id, token))
            .WithMessage("Задания не существует");
    }
}
