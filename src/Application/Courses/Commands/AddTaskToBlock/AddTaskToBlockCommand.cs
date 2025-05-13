using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.AddTaskToBlock;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(Id), typeof(Course))]
public record AddTaskToBlockCommand(Guid Id, Guid BlockId, Guid TaskId, int Weight = 0) : IRequest<TaskBlockResponse>;

public class AddTaskToBlockCommandHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<AddTaskToBlockCommand, TaskBlockResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<TaskBlockResponse> Handle(AddTaskToBlockCommand request, CancellationToken cancellationToken)
    {
        var taskBlock = await _context.TaskBlocks
            .Include(tb => tb.Tasks)
            .FirstAsync(tb => tb.Id == request.BlockId, cancellationToken);
        var task = await _context.Tasks
            .FirstAsync(t => t.Id == request.TaskId, cancellationToken);

        taskBlock.AddTask(task, request.Weight);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TaskBlockResponse>(taskBlock);
    }
}
