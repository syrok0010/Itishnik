namespace Itishnik.Application.Students;

public class TaskDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Text { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Task, TaskDto>();
        }
    }
}
