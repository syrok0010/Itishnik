namespace Itishnik.Application.Courses;

public record TaskListDto(Guid Id, string Name);

public record TaskBlockResponse(
    Guid Id,
    Guid CourseId,
    string Name,
    ICollection<TaskListDto> Tasks,
    ICollection<int> Weights,
    DateTime StartTime,
    DateTime EndTime,
    TimeSpan TimeAllowed);
