using Itishnik.Domain.Entities;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Application.Tasks;

public class TaskListResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public bool IsPublic { get; init; }
    
    public Guid? TeacherId { get; init; }
    public string TeacherFullName { get; init; } = null!;
    public string TeacherEmail { get; init; } = null!;
    
    public ICollection<Tag> Tags { get; init; } = null!;
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Task, TaskListResponse>()
                .ForMember(cr => cr.TeacherFullName, options => options.MapFrom(c => c.Teacher.FullName))
                .ForMember(cr => cr.TeacherEmail, options => options.MapFrom(c => c.Teacher.Email));
        }
    }
}
