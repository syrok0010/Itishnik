using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Queries.GetGradesOnCourse;

[Authorize(Policy = Policies.OwnerOrAdmin)]
[ResourceMetadata(nameof(Id), typeof(Course))]

public record GetGradesOnCourseQuery(Guid Id) : IRequest<StudentGradesResponse[]>;

public class GetGradesOnCourseQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetGradesOnCourseQuery, StudentGradesResponse[]>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<StudentGradesResponse[]> Handle(GetGradesOnCourseQuery request, CancellationToken cancellationToken)
    {
        return await _context.GradedCourses
            .Include(gc => gc.GradedTaskBlocks).ThenInclude(gtb => gtb.Solutions)
            .AsNoTracking()
            .Where(gc => gc.CourseId == request.Id)
            .ProjectTo<StudentGradesResponse>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);
    }
}
