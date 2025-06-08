using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.ChangeTaskBlockDescription;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(CourseId), typeof(Course))]
public record ChangeTaskBlockDescriptionCommand(Guid CourseId, Guid TaskBlockId, string? Description) 
    : IRequest<TaskBlockResponse>;

public class ChangeTaskBlockDescriptionCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<ChangeTaskBlockDescriptionCommand, TaskBlockResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<TaskBlockResponse> Handle(ChangeTaskBlockDescriptionCommand request, CancellationToken cancellationToken)
    {
        var taskBlock = await _context.TaskBlocks
            .Include(block => block.TasksEntries).ThenInclude(e => e.Task)
            .FirstAsync(tb => tb.Id == request.TaskBlockId, cancellationToken);
        taskBlock.Description = request.Description;
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TaskBlockResponse>(taskBlock);
    }
}
