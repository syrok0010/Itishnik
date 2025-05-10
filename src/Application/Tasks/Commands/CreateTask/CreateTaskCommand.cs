using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;

namespace Itishnik.Application.Tasks.Commands.CreateTask;

[Authorize(Roles = "Teacher")]
public record CreateTaskCommand(string Name, string Text, bool IsPublic, Guid? PreviousTaskId = null) : IRequest<TaskResponse[]>;

public class CreateTaskCommandHandler(IApplicationDbContext db, IMapper mapper, IUser currentUser) 
    : IRequestHandler<CreateTaskCommand, TaskResponse[]>
{
    private readonly IUser _currentUser = currentUser;
    private readonly IApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public async Task<TaskResponse[]> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var author = await _db.Teachers.FirstAsync(x => x.Id == _currentUser.Id, cancellationToken);
        Domain.Entities.Task? previousTask = null;
        if (request.PreviousTaskId is not null) 
            previousTask = await _db.Tasks
                .Include(x => x.FirstVersion)
                .FirstAsync(x => x.Id == request.PreviousTaskId, cancellationToken);
        
        var task = new Domain.Entities.Task(request.Name, request.Text, author, previousTask, request.IsPublic);
        await _db.Tasks.AddAsync(task, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return previousTask is null 
            ? [_mapper.Map<TaskResponse>(task)] 
            : await _db.Tasks
                .Where(x => x.Id == task.FirstVersion!.Id || (x.FirstVersion != null && task.FirstVersion!.Id == x.FirstVersion.Id))
                .ProjectTo<TaskResponse>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
    }
}
