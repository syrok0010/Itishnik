using Itishnik.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace Itishnik.Application.Students;

public class StudentCourseResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public int? Grade { get; init; }

    public ICollection<GradedTaskBlockDto> TaskBlocks { get; init; } = null!;
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GradedCourse, StudentCourseResponse>()
                .ForMember(c => c.Name, options => options.MapFrom(gc => gc.Course.Name))
                .ForMember(c => c.Description, options => options.MapFrom(gc => gc.Course.Description))
                .ForMember(c => c.TaskBlocks, options => options.MapFrom(gc => gc.GradedTaskBlocks));
        }
    }
}

public class GradedTaskBlockDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public TimeSpan TimeAllowed { get; init; }
    public DateTime? StudentStartTime { get; init; }
    
    public int? Grade { get; init; }
    public int TaskCount { get; init; }
    
    public ICollection<SolutionDto>? Solutions { get; init; }
    public ICollection<TaskDto>? Tasks { get; init; }
    

    protected class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GradedTaskBlock, GradedTaskBlockDto>()
                .ForMember(w => w.Name, options => options.MapFrom(gtb => gtb.TaskBlock.Name))
                .ForMember(w => w.Description, options => options.MapFrom(gtb => gtb.TaskBlock.Description))
                .ForMember(w => w.StartTime, options => options.MapFrom(gtb => gtb.TaskBlock.StartTime))
                .ForMember(w => w.EndTime, options => options.MapFrom(gtb => gtb.TaskBlock.EndTime))
                .ForMember(w => w.TimeAllowed, options => options.MapFrom(gtb => gtb.TaskBlock.TimeAllowed))
                .ForMember(w => w.StudentStartTime, options => options.MapFrom(gtb => gtb.StartTime))
                .ForMember(w => w.Grade, options => options.MapFrom(gtb => gtb.Grade))
                .ForMember(w => w.TaskCount, options => options.MapFrom(gtb => gtb.TaskBlock.Tasks.Count()))
                .ForMember(w => w.Solutions, options => options
                    .MapFrom(gtb => gtb.StartTime == null ? null : gtb.Solutions))
                .ForMember(w => w.Tasks, options => options
                    .MapFrom(gtb => gtb.StartTime == null ? null : gtb.TaskBlock.Tasks));
        }
    }
}

public class TaskDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Text { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Task, TaskDto>();
        }
    }
}

public class SolutionDto
{
    public Guid Id { get; init; }
    public string Text { get; init; } = null!;
    public int? Grade { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Solution, SolutionDto>();
        }
    }
}
