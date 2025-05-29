using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.AddTaskToBlock;

public class AddTaskToBlockCommandValidator : AbstractValidator<AddTaskToBlockCommand>
{
    public AddTaskToBlockCommandValidator(IApplicationDbContext context) 
    {
        RuleFor(x => x.Id)
            .MustAsync((id, token) => context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курса не существует");
        RuleFor(x => x.BlockId)
            .Cascade(CascadeMode.Stop)
            .MustAsync((cmd, id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && tb.CourseId == cmd.Id, token))
            .WithMessage("Работа не существует или не принадлежит заданному курсу")
            .MustAsync((id, token) => context.TaskBlocks.AnyAsync(tb => tb.Id == id && !tb.IsPublic, token))
            .WithMessage("Нельзя добавить задачу в опубликованную работу");
        RuleFor(x => x.TaskId)
            .MustAsync((id, token) => context.Tasks.AnyAsync(t => t.Id == id, token))
            .WithMessage("Задания не существует");
        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Вес не может быть отрицательным");
    }
}
