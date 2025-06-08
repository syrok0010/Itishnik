using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.PublishTaskBlock;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(CourseId), typeof(Course))]
public record PublishTaskBlockCommand(Guid CourseId, Guid TaskBlockId) : IRequest<TaskBlockResponse>;

public class PublishTaskBlockCommandHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<PublishTaskBlockCommand, TaskBlockResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<TaskBlockResponse> Handle(PublishTaskBlockCommand request, CancellationToken cancellationToken)
    {
        var block = await _context.TaskBlocks
            .Include(block => block.TasksEntries).ThenInclude(e => e.Task)
            .Include(tb => tb.Course).ThenInclude(c => c.GradedCourses).ThenInclude(gc => gc.Student)
            .FirstAsync(tb => tb.Id == request.TaskBlockId, cancellationToken);
        block.IsPublic = true;
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TaskBlockResponse>(block);
    }
}
