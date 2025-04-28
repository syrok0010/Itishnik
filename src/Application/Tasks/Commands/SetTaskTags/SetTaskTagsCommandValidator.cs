using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Tasks.Commands.SetTaskTags;

public class SetTaskTagsCommandValidator : AbstractValidator<SetTaskTagsCommand>
{
    public SetTaskTagsCommandValidator(IApplicationDbContext db)
    {
        RuleFor(c => c.TaskId)
            .MustAsync((taskId, ct) => db.Tasks.AnyAsync(x => x.Id == taskId, ct))
            .WithMessage("Заданная задача не существует");

        RuleFor(c => c.TagIds)
            .MustAsync(async (tagIds, ct) => await db.Tags.CountAsync(t => tagIds.Contains(t.Id), ct) == tagIds.Count)
            .WithMessage("Не все указанные тэги существуют");
    }
}
