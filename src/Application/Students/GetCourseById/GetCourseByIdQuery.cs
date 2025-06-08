using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Students.GetCourseById;

[Authorize(Roles = Roles.Student)]
[ResourceMetadata(nameof(Id), typeof(GradedCourse))]
public record GetCourseByIdQuery(Guid Id) : IRequest<StudentCourseResponse>;

public class GetCourseByIdQueryHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<GetCourseByIdQuery, StudentCourseResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<StudentCourseResponse> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var gradedCourse = await _context.GradedCourses
            .Include(gc => gc.Course)
            .Include(gc => gc.GradedTaskBlocks).ThenInclude(gtb => gtb.Solutions).ThenInclude(s => s.Task)
            .Include(gc => gc.GradedTaskBlocks).ThenInclude(gtb => gtb.TaskBlock).ThenInclude(tb => tb.TasksEntries)
            .AsNoTracking()
            .FirstAsync(gc => gc.Id == request.Id, cancellationToken);

        return _mapper.Map<StudentCourseResponse>(gradedCourse);
    }
}
