using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace Itishnik.Application.Courses.Commands.DeleteTaskFromBlock;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(Id), typeof(Course))]
public record DeleteTaskFromBlockCommand(Guid Id, Guid BlockId, Guid TaskId) : IRequest;

public class DeleteTaskFromBlockCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<DeleteTaskFromBlockCommand>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task Handle(DeleteTaskFromBlockCommand request, CancellationToken cancellationToken)
    {
        var taskBlock = await _context.TaskBlocks
            .Include(tb => tb.TasksEntries)
            .FirstAsync(tb => tb.Id == request.BlockId, cancellationToken);
        var task = await _context.Tasks
            .FirstAsync(t => t.Id == request.TaskId, cancellationToken);

        taskBlock.RemoveTask(task);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
