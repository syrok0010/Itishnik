using Itishnik.Application.Courses;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Application.Common.Mappings;

public static class TasksMappings
{
    public static TaskListDto ToTaskListDto(this Task task)
    {
        return new TaskListDto(task.Id, task.Name);
    }
}
