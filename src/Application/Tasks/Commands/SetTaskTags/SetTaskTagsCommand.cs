using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Application.Tasks.Queries;
using Itishnik.Domain.Constants;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Application.Tasks.Commands.SetTaskTags;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(TaskId), typeof(Task))]
public record SetTaskTagsCommand(Guid TaskId, ICollection<Guid> TagIds) : IRequest<TaskResponse[]>;

public class SetTaskTagsCommandHandler(IApplicationDbContext db, IMapper mapper) : IRequestHandler<SetTaskTagsCommand, TaskResponse[]>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public async Task<TaskResponse[]> Handle(SetTaskTagsCommand request, CancellationToken cancellationToken)
    {
        var versionChain = await _db.Tasks
            .Include(t => t.Tags)
            .GetTaskChain(_db, request.TaskId)
            .OrderBy(t => t.Created)
            .ToListAsync(cancellationToken);
        var tags = await _db.Tags
            .Where(tag => request.TagIds.Contains(tag.Id))
            .ToListAsync(cancellationToken);
        
        foreach (var version in versionChain) 
            version.SetTags(tags);
        
        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TaskResponse[]>(versionChain);
    }
}
