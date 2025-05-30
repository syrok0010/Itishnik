using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses;

public class CourseListResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public int StudentsCount { get; init; }
    public int TaskBlocksCount { get; init; }
    public string? Description { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Course, CourseListResponse>()
                .ForMember(clr => clr.StudentsCount, options => options.MapFrom(c => c.GradedCourses.Count()))
                .ForMember(clr => clr.TaskBlocksCount, options => options.MapFrom(c => c.TaskBlocks.Count()));
        }
    }
}
