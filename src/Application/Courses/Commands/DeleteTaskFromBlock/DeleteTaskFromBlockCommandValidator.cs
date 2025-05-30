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
            .Cascade(CascadeMode.Stop)
            .MustAsync((cmd, id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.Id, token))
            .WithMessage("Работа не существует или не принадлежит заданному курсу")
            .MustAsync((id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && !tb.IsPublic, token))
            .WithMessage("Нельзя удалить задачу из опубликованного блока");
        RuleFor(x => x.TaskId)
            .MustAsync((id, token) => context.Tasks.AnyAsync(t => t.Id == id, token))
            .WithMessage("Задания не существует");
    }
}
