using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Application.Tasks.Commands.EditReferenceSolution;

[Authorize(Policy = Policies.OwnerOrAdmin)]
[ResourceMetadata(nameof(TaskId), typeof(Task))]
public record EditReferenceSolutionCommand(Guid TaskId, string Text) : IRequest<TaskResponse>;

public class EditReferenceSolutionCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<EditReferenceSolutionCommand, TaskResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<TaskResponse> Handle(EditReferenceSolutionCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.FirstVersion)
            .Include(t => t.Teacher)
            .Include(t => t.Tags)
            .FirstAsync(t => t.Id == request.TaskId, cancellationToken);
        task.ReferenceSolutionText = request.Text;
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TaskResponse>(task);
    }
}
