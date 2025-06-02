using Itishnik.Domain.Entities;

namespace Itishnik.Application.Students;

public class FeedbackDto
{
    public Guid TaskBlockId { get; init; }
    public Guid StudentId { get; init; }

    public string Feedback { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<GradedTaskBlock, FeedbackDto>();
        }
    }
}
