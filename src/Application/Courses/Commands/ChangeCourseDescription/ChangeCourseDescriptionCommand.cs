using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.ChangeCourseDescription;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(Id), typeof(Course))]
public record ChangeCourseDescriptionCommand(Guid Id, string? Description) : IRequest<CourseResponse>;

public class ChangeCourseDescriptionCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<ChangeCourseDescriptionCommand, CourseResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<CourseResponse> Handle(ChangeCourseDescriptionCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.TaskBlocks)
            .FirstAsync(c => c.Id == request.Id, cancellationToken);
        course.Description = request.Description;
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CourseResponse>(course);
    }
}
