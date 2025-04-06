namespace Itishnik.Application.Courses;

public record CourseListResponse(Guid Id, string Name, int StudentsCount, string? Description = null);
