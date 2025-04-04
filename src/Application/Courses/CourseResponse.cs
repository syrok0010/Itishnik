namespace Itishnik.Application.Courses;

public record CourseResponse(
    Guid Id, 
    string Name,
    Guid TeacherId,
    string TeacherFullName,
    string TeacherEmail,
    ICollection<TaskBlockResponse> TaskBlocks,
    string? Description = null);
