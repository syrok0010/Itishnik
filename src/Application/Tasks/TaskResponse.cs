using Itishnik.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace Itishnik.Application.Tasks;

public class TaskResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public bool IsPublic { get; init; }
    public string Text { get; init; } = null!;
    
    public Guid? RightSolutionId { get; init; }
    public Guid? TeacherId { get; init; }
    
    public Guid? PreviousVersionId { get; init; }
    public Guid? FirstVersionId { get; init; }

    public ICollection<Tag> Tags { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Task, TaskResponse>();
        }
    }
}
