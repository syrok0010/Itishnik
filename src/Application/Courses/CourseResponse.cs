using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses;

public class CourseResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public Guid TeacherId { get; init; }
    public string TeacherFullName { get; init; } = null!;
    public string TeacherEmail { get; init; } = null!;
    public ICollection<TaskBlockResponse> TaskBlocks { get; init; } = null!;
    public string? Description { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Course, CourseResponse>()
                .ForMember(cr => cr.TeacherFullName, options => options.MapFrom(c => c.Teacher.FullName))
                .ForMember(cr => cr.TeacherEmail, options => options.MapFrom(c => c.Teacher.Email))
                .ForMember(cr => cr.TaskBlocks, options => options.MapFrom(c => c.TaskBlocks));
        }
    }
}
