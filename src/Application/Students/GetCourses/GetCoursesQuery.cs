using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Mappings;
using Itishnik.Application.Common.Models;
using Itishnik.Application.Courses;

namespace Itishnik.Application.Students.GetCourses;

public record GetCoursesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<GradedCourseResponse>>;

public class GetCoursesQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IUser user)
    : IRequestHandler<GetCoursesQuery, PaginatedList<GradedCourseResponse>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;
    
    public async Task<PaginatedList<GradedCourseResponse>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        return await _context.GradedCourses
            .AsNoTracking()
            .Include(gc => gc.Course)
            .Where(gc => gc.StudentId == _user.Id)
            .ProjectTo<GradedCourseResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
