using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses;

public class StudentGradesResponse
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int?[] Grades { get; set; } = null!;
    public int? CourseGrade { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GradedCourse, StudentGradesResponse>()
                .ForMember(sgr => sgr.FullName, options => options.MapFrom(gc => gc.Student.FullName))
                .ForMember(sgr => sgr.Email, options => options.MapFrom(gc => gc.Student.Email))
                .ForMember(sgr => sgr.Grades,
                    options => options.MapFrom(gc => gc.GradedTaskBlocks.Select(gtb => gtb.Grade)))
                .ForMember(sgr => sgr.CourseGrade,
                    options => options.MapFrom(gc => gc.Grade));
        }
    }
}
