using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Mappings;

namespace Itishnik.Application.Courses.Queries;

public record GetCourseByIdQuery(Guid Id) : IRequest<CourseResponse?>;

public class GetCourseByIdQueryHandler(IApplicationDbContext context) 
    : IRequestHandler<GetCourseByIdQuery, CourseResponse?>
{
    private readonly IApplicationDbContext _context = context;
    
    public async Task<CourseResponse?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(x => x.Teacher)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return course?.ToCourseResponse();
    }
}
