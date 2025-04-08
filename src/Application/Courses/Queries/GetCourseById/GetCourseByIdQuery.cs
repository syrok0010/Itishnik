using Itishnik.Application.Common.Interfaces;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Queries.GetCourseById;

public record GetCourseByIdQuery(Guid Id) : IRequest<CourseResponse?>;

public class GetCourseByIdQueryHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<GetCourseByIdQuery, CourseResponse?>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<CourseResponse?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(x => x.Teacher)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return course == null ? null : _mapper.Map<Course, CourseResponse>(course);
    }
}
