namespace Itishnik.Application.Courses;

public record CourseListResponse(Guid Id, string Name, string? Description = null);
