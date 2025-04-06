using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Mappings;
using Itishnik.Application.Common.Models;

namespace Itishnik.Application.Courses.Queries.GetCourseList;

public record GetCoursesListQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<CourseListResponse>>;

public class CreateCoursesListQueryHandler(IApplicationDbContext context) 
    : IRequestHandler<GetCoursesListQuery, PaginatedList<CourseListResponse>>
{
    private readonly IApplicationDbContext _context = context;
    
    public async Task<PaginatedList<CourseListResponse>> Handle(GetCoursesListQuery request, CancellationToken cancellationToken)
    {
        return await _context.Courses
            .Select(x => x.ToCourseListResponse())
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
