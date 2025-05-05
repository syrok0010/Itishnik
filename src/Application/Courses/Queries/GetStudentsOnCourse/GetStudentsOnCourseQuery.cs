using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Queries.GetStudentsOnCourse;

[Authorize(Policy = Policies.OwnerOrAdmin)]
[ResourceMetadata(nameof(Id), typeof(Course))]
public record GetStudentsOnCourseQuery(Guid Id) : IRequest<CourseStudentListResponse>;

public class GetStudentsOnCourseQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetStudentsOnCourseQuery, CourseStudentListResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<CourseStudentListResponse> Handle(GetStudentsOnCourseQuery request, CancellationToken cancellationToken)
    {
        var students = await _context.Students
            .AsNoTracking()
            .Where(s => s.GradedCourses.Any(gc => gc.CourseId == request.Id))
            .ProjectTo<StudentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return new CourseStudentListResponse { Students = students };
    }
}
