using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.ChangeTaskBlockTimeline;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(CourseId), typeof(Course))]
public record ChangeTaskBlockTimelineCommand(
    Guid CourseId,
    Guid TaskBlockId,
    DateTime StartTime,
    DateTime EndTime,
    TimeSpan? TimeAllowed) : IRequest<TaskBlockResponse>;

public class ChangeTaskBlockTimelineCommandHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<ChangeTaskBlockTimelineCommand, TaskBlockResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<TaskBlockResponse> Handle(ChangeTaskBlockTimelineCommand request, CancellationToken cancellationToken)
    {
        var taskBlock = await _context.TaskBlocks
            .Include(block => block.TasksEntries).ThenInclude(e => e.Task)
            .FirstAsync(block => block.Id == request.TaskBlockId, cancellationToken);
        taskBlock.ChangeTimes(request.StartTime, request.EndTime, request.TimeAllowed);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TaskBlockResponse>(taskBlock);
    }
}
