using Itishnik.Domain.Entities;

namespace Itishnik.Application.Tasks;

public class TaskResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public bool IsPublic { get; init; }
    public string Text { get; init; } = null!;
    
    public Guid? RightSolutionId { get; init; }
    public Guid? TeacherId { get; init; }
    public string TeacherFullName { get; init; } = null!;
    public string TeacherEmail { get; init; } = null!;

    public ICollection<Tag> Tags { get; init; } = null!;
    
    public Guid? FirstTaskId { get; init; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Task, TaskResponse>()
                .ForMember(cr => cr.TeacherFullName, options => options.MapFrom(c => c.Teacher.FullName))
                .ForMember(cr => cr.TeacherEmail, options => options.MapFrom(c => c.Teacher.Email))
                .ForMember(cr => cr.FirstTaskId, options => options.MapFrom(c => c.FirstVersion == null ? (Guid?)null : c.FirstVersion.Id));
        }
    }
}
