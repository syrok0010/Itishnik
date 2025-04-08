using Itishnik.Application.Courses;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Application.Common.Mappings;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<Task, TaskListDto>();
    }
}
