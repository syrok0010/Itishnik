using Itishnik.Application.Common.Interfaces;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Application.Tasks.Queries;

public static class DbTasksExtensions
{
    public static IQueryable<Task> GetTaskChain(this IQueryable<Task> tasks, IApplicationDbContext db, Guid taskId)
    {
        return tasks
            .Where(t =>
                db.Tasks
                    .Where(original => original.Id == taskId)
                    .Select(original => original.FirstVersion == null ? original.Id : original.FirstVersion.Id)
                    .Any(firstVersionId =>
                        t.Id == firstVersionId || (t.FirstVersion != null && t.FirstVersion.Id == firstVersionId)
                    )
            )
            .OrderBy(t => t.Created);
    }
}
