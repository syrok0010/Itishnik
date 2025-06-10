using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Application.Students;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Queries.GetGradedTaskBlock;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(CourseId), typeof(Course))]
public record GetGradedTaskBlockQuery(Guid CourseId, Guid TaskBlockId, Guid StudentId) : IRequest<GradedTaskBlockDto?>;

public class GetGradedTaskBlockQueryHandler(IApplicationDbContext db, IMapper mapper) : IRequestHandler<GetGradedTaskBlockQuery, GradedTaskBlockDto?>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public Task<GradedTaskBlockDto?> Handle(GetGradedTaskBlockQuery request, CancellationToken cancellationToken)
    {
        return _db.GradedTaskBlocks
            .Where(gtb => gtb.StudentId == request.StudentId && gtb.TaskBlockId == request.TaskBlockId)
            .ProjectTo<GradedTaskBlockDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
