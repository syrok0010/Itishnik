using Itishnik.Domain.Entities;

namespace Itishnik.Application.Students;

public class SolutionDto
{
    public Guid Id { get; init; }
    public string Text { get; init; } = null!;
    public int? Grade { get; init; }

    public int Weight { get; init; }
    public int Position { get; init; }
    
    public TaskDto Task { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Solution, SolutionDto>()
                .ForMember(s => s.Task, opt => opt.MapFrom(s => s.Task));
        }
    }
}
