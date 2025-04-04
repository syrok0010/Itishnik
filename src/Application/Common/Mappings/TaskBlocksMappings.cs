using Itishnik.Application.Courses;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Common.Mappings;

public static class TaskBlocksMappings
{
    public static TaskBlockResponse ToTaskBlockResponse(this TaskBlock taskBlock)
    {
        return new TaskBlockResponse(
            taskBlock.Id,
            taskBlock.CourseId,
            taskBlock.Name,
            taskBlock.Tasks.Select(t => t.ToTaskListDto()).ToList(),
            taskBlock.Weights.ToList(),
            taskBlock.StartTime,
            taskBlock.EndTime,
            taskBlock.TimeAllowed
        );
    }
}
