using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.CreateTaskBlock;

[Authorize(Policy = Policies.CourseOwner)]
public record CreateTaskBlockCommand(
    Guid CourseId,
    string Name,
    IList<Guid> TaskIds,
    IList<int> Weights,
    DateTime StartTime,
    DateTime EndTime,
    TimeSpan TimeAllowed,
    string? Description = null) : IRequest<TaskBlockResponse>;

public class CreateTaskBlockCommandHandler(IApplicationDbContext context, IMapper mapper, IUser currentUser) :
    IRequestHandler<CreateTaskBlockCommand, TaskBlockResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _currentUser = currentUser;
    
    public async Task<TaskBlockResponse> Handle(CreateTaskBlockCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses.FirstAsync(x => x.Id == request.CourseId, cancellationToken);
        var taskBlock = new TaskBlock(
            request.Name, 
            course,
            request.StartTime,
            request.EndTime,
            request.TimeAllowed,
            request.Description);
        for (var i = 0; i < request.TaskIds.Count; i++)
        {
            var task = await _context.Tasks.FirstAsync(x => x.Id == request.TaskIds[i], cancellationToken);
            taskBlock.AddTask(task, request.Weights[i]);
        }
        course.AddTaskBlock(taskBlock);
        return _mapper.Map<TaskBlockResponse>(taskBlock);
    }
}
