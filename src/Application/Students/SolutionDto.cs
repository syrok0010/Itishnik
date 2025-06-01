using Itishnik.Domain.Entities;

namespace Itishnik.Application.Students;

public class SolutionDto
{
    public Guid Id { get; init; }
    public string Text { get; init; } = null!;
    public int? Grade { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Solution, SolutionDto>();
        }
    }
}
