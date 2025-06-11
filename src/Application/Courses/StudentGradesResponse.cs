using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses;

public class StudentGradesResponse
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public GradedTaskBlockResponse[] Grades { get; set; } = null!;
    public int? CourseGrade { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GradedCourse, StudentGradesResponse>()
                .ForMember(sgr => sgr.FullName, options => options.MapFrom(gc => gc.Student.FullName))
                .ForMember(sgr => sgr.Email, options => options.MapFrom(gc => gc.Student.Email))
                .ForMember(sgr => sgr.CourseGrade,
                    options => options.MapFrom(gc => gc.Grade))
                .ForMember(sgr => sgr.Grades, options => options.MapFrom(gc => gc.GradedTaskBlocks.OrderBy(gtb => gtb.TaskBlock.StartTime)));
        }
    }
}

public class GradedTaskBlockResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int? Grade { get; set; }
    public bool SolutionsIsEmpty { get; set; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GradedTaskBlock, GradedTaskBlockResponse>()
                .ForMember(gt => gt.Name, options => options.MapFrom(gt => gt.TaskBlock.Name))
                .ForMember(gt => gt.SolutionsIsEmpty, options => options.MapFrom(gt => !gt.Solutions.Any()));
        }
    }
}
