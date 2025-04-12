using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Tasks.Commands.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator(IApplicationDbContext db, IUser currentUser)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255)
            .WithMessage("Длина названия не должна превышать 255 символов");
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(5000)
            .WithMessage("Длина задания не должна превышать 5000 символов");

        RuleFor(x => x.Name)
            .MustAsync((name, cancellationToken) =>
                db.Tasks.AllAsync(t => t.TeacherId != currentUser.Id || t.Name != name, cancellationToken)
            )
            .WithMessage("Задача с таким названием уже существует у этого преподавателя.")
            .Unless(x => x.PreviousTaskId is not null);
        
        RuleFor(x => x.Text)
            .MustAsync((cmd, text, cancellationToken) =>
                db.Tasks.AllAsync(t => t.Id != cmd.PreviousTaskId!.Value || t.Text != text, cancellationToken)
            )
            .WithMessage("Условие должно отличаться от прошлой версии.")
            .Unless(x => x.PreviousTaskId is null);

        RuleFor(x => x.PreviousTaskId)
            .MustAsync((prevId, cancellationToken) =>
                db.Tasks.AnyAsync(t => t.Id == prevId!.Value && t.TeacherId == currentUser.Id, cancellationToken)
            )
            .Unless(x => x.PreviousTaskId is null)
            .WithMessage("Указанная предыдущая версия задачи не найдена или не принадлежит вам.")
            .DependentRules(() =>
            {
                RuleFor(x => x.Name)
                    .MustAsync((command, name, cancellationToken) =>
                        db.Tasks.AnyAsync(t => t.Id == command.PreviousTaskId!.Value && name == t.Name,
                            cancellationToken))
                    .WithMessage("Имя новой версии должно совпадать с предыдущим.")
                    .Unless(x => x.PreviousTaskId is null);
            });

        RuleFor(x => x.PreviousTaskId)
            .MustAsync((prevId, cancellationToken) =>
                db.Tasks.AllAsync(t => t.PreviousVersion == null || t.PreviousVersion!.Id != prevId!.Value, cancellationToken)
            )
            .Unless(x => x.PreviousTaskId is null)
            .WithMessage("У указанной предыдущей задачи уже есть следующая версия.");
    }
}
