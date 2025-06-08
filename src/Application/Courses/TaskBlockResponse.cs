using Itishnik.Domain.Entities;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Application.Courses;

public class TaskListDto
{
    public Guid Id { get; init; }
    public int Weight { get; init; }
    public int Position { get; init; }
    public string Name { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TaskBlockEntry, TaskListDto>()
                .ForMember(tl => tl.Id, o => o.MapFrom(e => e.TaskId))
                .ForMember(tl => tl.Name, o => o.MapFrom(e => e.Task.Name));
        }
    }
}

public class TaskBlockResponse
{
    public Guid Id { get; init; }
    public Guid CourseId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public ICollection<TaskListDto> Tasks { get; init; } = null!;
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public TimeSpan? TimeAllowed { get; init; }
    public bool IsPublic { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TaskBlock, TaskBlockResponse>()
                .ForMember(tbr => tbr.Tasks, options => options.MapFrom(tb => tb.TasksEntries.OrderBy(x => x.Position)));
        }
    }
}
