using Itishnik.Application.Courses;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Common.Mappings;

public static class CourseMappings
{
    public static CourseResponse ToCourseResponse(this Course course)
    {
        return new CourseResponse(
            course.Id,
            course.Name,
            course.TeacherId,
            course.Teacher.FullName,
            course.Teacher.Email ?? throw new ArgumentNullException(nameof(course.Teacher.Email)),
            course.TaskBlocks
                .Select(tb => tb.ToTaskBlockResponse())
                .ToList(),
            course.Description
        );
    }
}
