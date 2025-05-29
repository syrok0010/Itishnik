using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Mappings;
using Itishnik.Application.Common.Models;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;

namespace Itishnik.Application.Students.GetCourses;

[Authorize(Roles = Roles.Student)]
public record GetCoursesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<GradedCourseResponse>>;

public class GetCoursesQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetCoursesQuery, PaginatedList<GradedCourseResponse>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<PaginatedList<GradedCourseResponse>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        return await _context.GradedCourses
            .AsNoTracking()
            .ProjectTo<GradedCourseResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
