using Itishnik.Domain.Entities;

namespace Itishnik.Application.Students;

public class GradedCourseResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public int Grade { get; init; }
    public DateTime NearestTaskBlockStart { get; init; }
    public DateTime NearestTaskBlockEnd { get; init; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GradedCourse, GradedCourseResponse>()
                .ForMember(gcr => gcr.Name, options => options.MapFrom(gc => gc.Course.Name))
                .ForMember(gcr => gcr.NearestTaskBlockStart, options => options.Condition(gc => gc.GradedTaskBlocks
                            .FirstOrDefault(gtb => DateTime.Now <= gtb.TaskBlock.EndTime) is not null))
                .ForMember(gcr => gcr.NearestTaskBlockEnd, options => options.Condition(gc => gc.GradedTaskBlocks
                            .FirstOrDefault(gtb => DateTime.Now <= gtb.TaskBlock.EndTime) is not null))
                .ForMember(gcr => gcr.Grade, options => options.Condition(gc => gc.GradedTaskBlocks
                    .FirstOrDefault(gtb => DateTime.Now <= gtb.TaskBlock.EndTime) is null));
        }
    }
}
