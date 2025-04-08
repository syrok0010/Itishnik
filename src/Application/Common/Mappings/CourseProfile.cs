using Itishnik.Application.Courses;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Common.Mappings;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseResponse>()
            .ForMember(cr => cr.TeacherFullName, options => options.MapFrom(c => c.Teacher.FullName))
            .ForMember(cr => cr.TeacherEmail, options => options.MapFrom(c => c.Teacher.Email))
            .ForMember(cr => cr.TaskBlocks, options => options.MapFrom(c => c.TaskBlocks));
        CreateMap<Course, CourseListResponse>()
            .ForMember(clr => clr.StudentsCount, options => options.MapFrom(c => c.Students.Count()));
    }
}
