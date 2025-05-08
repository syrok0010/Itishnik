using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Application.Tasks.Queries;
using Itishnik.Domain.Constants;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Application.Tasks.Commands.PublishTask;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(TaskId), typeof(Task))]
public record PublishTaskCommand(Guid TaskId) : IRequest<TaskResponse[]>;

public class PublishTaskCommandHandler(IApplicationDbContext db, IMapper mapper) : IRequestHandler<PublishTaskCommand, TaskResponse[]>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public async Task<TaskResponse[]> Handle(PublishTaskCommand request, CancellationToken cancellationToken)
    {
        var versionChain = await _db.Tasks
            .Include(t => t.Tags)
            .GetTaskChain(_db, request.TaskId)
            .OrderBy(t => t.Created)
            .ToListAsync(cancellationToken);

        foreach (var task in versionChain)
            task.IsPublic = true;

        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TaskResponse[]>(versionChain);
    }
}
