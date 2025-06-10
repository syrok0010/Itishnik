using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Application.Courses;
using Itishnik.Domain.Constants;

namespace Itishnik.Application.Students.GetStudentAllGrades;

[Authorize(Roles = Roles.Student)]
public record GetStudentAllGradesQuery : IRequest<StudentGradesResponse[]>;

public class GetStudentAllGradesQueryHandler(IApplicationDbContext context, IMapper mapper, IUser user) : IRequestHandler<GetStudentAllGradesQuery, StudentGradesResponse[]>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _user = user;

    public async Task<StudentGradesResponse[]> Handle(GetStudentAllGradesQuery request, CancellationToken cancellationToken)
    {
        return await _context.GradedCourses
            .Include(gc => gc.GradedTaskBlocks.OrderBy(gtb => gtb.TaskBlock.StartTime))
            .AsNoTracking()
            .Where(gc => gc.StudentId == _user.Id)
            .ProjectTo<StudentGradesResponse>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);
    }
}

