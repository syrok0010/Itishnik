using Itishnik.Domain.Entities;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Application.Courses;

public class TaskListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Task, TaskListDto>();
        }
    }
}

public class TaskBlockResponse
{
    public Guid Id { get; init; }
    public Guid CourseId { get; init; }
    public string Name { get; init; } = null!;
    public ICollection<TaskListDto> Tasks { get; init; } = null!;
    public ICollection<int> Weights { get; init; } = null!;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public TimeSpan TimeAllowed { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TaskBlock, TaskBlockResponse>()
                .ForMember(tbr => tbr.Tasks, options => options.MapFrom(tb => tb.Tasks))
                .ForMember(tbr => tbr.Weights, options => options.MapFrom(tb => tb.Weights));
        }
    }
}
