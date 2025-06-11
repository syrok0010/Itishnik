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

    public async Task<GradedTaskBlockDto?> Handle(GetGradedTaskBlockQuery request, CancellationToken cancellationToken)
    {
        var gtb = await _db.GradedTaskBlocks
            .Include(x => x.Solutions).ThenInclude(x => x.Task)
            .Include(x => x.TaskBlock).ThenInclude(x => x.TasksEntries)
            .Where(gtb => gtb.StudentId == request.StudentId && gtb.TaskBlockId == request.TaskBlockId)
            .FirstOrDefaultAsync(cancellationToken);
        return _mapper.Map<GradedTaskBlockDto>(gtb);
    }
}
