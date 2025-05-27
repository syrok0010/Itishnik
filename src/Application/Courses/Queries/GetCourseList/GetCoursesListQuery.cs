using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Mappings;
using Itishnik.Application.Common.Models;

namespace Itishnik.Application.Courses.Queries.GetCourseList;

public record GetCoursesListQuery(bool Ascending, int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<CourseListResponse>>;

public class GetCoursesListQueryHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<GetCoursesListQuery, PaginatedList<CourseListResponse>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public Task<PaginatedList<CourseListResponse>> Handle(GetCoursesListQuery request, CancellationToken cancellationToken)
    {
        var courses = _context.Courses.AsNoTracking();
        courses = request.Ascending 
            ? courses.OrderBy(x => x.Name) 
            : courses.OrderByDescending(x => x.Name);
        
        return courses
            .ProjectTo<CourseListResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
