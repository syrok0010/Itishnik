using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Tasks.Commands.PublishTask;

public class PublishTaskCommandValidator : AbstractValidator<PublishTaskCommand>
{
    public PublishTaskCommandValidator(IApplicationDbContext db)
    {
        RuleFor(c => c.TaskId)
            .MustAsync((taskId, ct) => db.Tasks.AnyAsync(x => x.Id == taskId && !x.IsPublic, ct))
            .WithMessage("Заданная задача не существует или уже опубликованаы");
    }
}
