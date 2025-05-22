using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Mappings;
using Itishnik.Application.Common.Models;

namespace Itishnik.Application.Courses.Queries.GetCourseList;

public record GetCoursesListQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<CourseListResponse>>;

public class GetCoursesListQueryHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<GetCoursesListQuery, PaginatedList<CourseListResponse>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public Task<PaginatedList<CourseListResponse>> Handle(GetCoursesListQuery request, CancellationToken cancellationToken)
    {
        return _context.Courses
            .AsNoTracking()
            .ProjectTo<CourseListResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
