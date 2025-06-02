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
public record GetFeedbacksQuery(Guid CourseId, Guid TaskBlockId, int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<FeedbackDto>>;

public class GetFeedbacksQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetFeedbacksQuery, PaginatedList<FeedbackDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<PaginatedList<FeedbackDto>> Handle(GetFeedbacksQuery request, CancellationToken cancellationToken)
    {
        return await _context.GradedTaskBlocks
            .AsNoTracking()
            .Where(b => b.TaskBlockId == request.TaskBlockId)
            .ProjectTo<FeedbackDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
