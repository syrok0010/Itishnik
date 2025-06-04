using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Mappings;
using Itishnik.Application.Common.Models;
using Itishnik.Application.Common.Security;
using Itishnik.Application.Students;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Queries.GetFeedbacks;

[Authorize(Policy = Policies.OwnerOrAdmin)]
[ResourceMetadata(nameof(CourseId), typeof(Course))]
public record GetFeedbacksQuery(Guid CourseId, Guid TaskBlockId) : IRequest<List<string>>;

public class GetFeedbacksQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetFeedbacksQuery, List<string>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<List<string>> Handle(GetFeedbacksQuery request, CancellationToken cancellationToken)
    {
        return await _context.GradedTaskBlocks
            .AsNoTracking()
            .Where(b => b.TaskBlockId == request.TaskBlockId && !string.IsNullOrWhiteSpace(b.Feedback))
            .Select(b => b.Feedback!)
            .ToListAsync(cancellationToken);
    }
}
