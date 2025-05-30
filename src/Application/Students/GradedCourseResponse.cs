using Itishnik.Domain.Entities;

namespace Itishnik.Application.Students;

public class GradedCourseResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;

    public int? Grade { get; init; }
    public DateTime? NearestTaskBlockStart { get; init; }
    public DateTime? NearestTaskBlockEnd { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GradedCourse, GradedCourseResponse>()
                .ForMember(gcr => gcr.Name, options => options.MapFrom(gc => gc.Course.Name))
                .ForMember(gcr => gcr.Grade, options => options.MapFrom(gc => gc.Grade))
                .ForMember(gcr => gcr.NearestTaskBlockStart, options => options.MapFrom(gc =>
                    gc.Grade.HasValue
                        ? null
                        : gc.GradedTaskBlocks
                            .Where(gtb => TimeProvider.System.GetUtcNow() <= gtb.TaskBlock.EndTime)
                            .OrderBy(gtb => gtb.TaskBlock.StartTime)
                            .Select(gtb => gtb.TaskBlock.StartTime)
                            .FirstOrDefault()))
                .ForMember(gcr => gcr.NearestTaskBlockEnd, options => options.MapFrom(gc =>
                    gc.Grade.HasValue
                        ? null
                        : gc.GradedTaskBlocks
                            .Where(gtb => TimeProvider.System.GetUtcNow() <= gtb.TaskBlock.EndTime)
                            .OrderBy(gtb => gtb.TaskBlock.StartTime)
                            .Select(gtb => gtb.TaskBlock.EndTime)
                            .FirstOrDefault()));
        }
    }
}
