using Itishnik.Domain.Entities;

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
