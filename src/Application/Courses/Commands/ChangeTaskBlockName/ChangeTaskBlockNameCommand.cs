using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.ChangeTaskBlockName;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(CourseId), typeof(Course))]
public record ChangeTaskBlockNameCommand(Guid CourseId, Guid TaskBlockId, string Name) : IRequest<TaskBlockResponse>;

public class ChangeTaskBlockNameCommandHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<ChangeTaskBlockNameCommand, TaskBlockResponse>
{
    private readonly IMapper _mapper = mapper;
    private readonly IApplicationDbContext _context = context;  
    
    public async Task<TaskBlockResponse> Handle(ChangeTaskBlockNameCommand request, CancellationToken cancellationToken)
    {
        var taskBlock = await _context.TaskBlocks
            .Include(block => block.TasksEntries).ThenInclude(e => e.Task)
            .FirstAsync(tb => tb.Id == request.TaskBlockId, cancellationToken);
        taskBlock.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TaskBlockResponse>(taskBlock);
    }
}
