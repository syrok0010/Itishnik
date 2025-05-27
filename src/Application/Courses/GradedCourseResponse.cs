using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses;

public class GradedCourseResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public int Grade { get; init; }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GradedCourse, GradedCourseResponse>()
                .ForMember(gcr => gcr.Name, options => options.MapFrom(gc => gc.Course.Name));
        }
    }
}
