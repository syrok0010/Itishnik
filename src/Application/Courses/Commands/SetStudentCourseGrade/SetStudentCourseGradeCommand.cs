using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace Itishnik.Application.Courses.Commands.SetStudentCourseGrade;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(CourseId), typeof(Course))]
public record SetStudentCourseGradeCommand(Guid CourseId, Guid StudentId, int Grade) :  IRequest;

public class SetStudentCourseGradeCommandHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<SetStudentCourseGradeCommand>
{
    private readonly IMapper _mapper = mapper;
    private readonly IApplicationDbContext _context = context;  
    
    public async Task Handle(SetStudentCourseGradeCommand request, CancellationToken cancellationToken)
    {
        var gradedCourse = await _context.GradedCourses.FirstAsync(gc =>
            gc.CourseId == request.CourseId && gc.StudentId == request.StudentId, cancellationToken);
        gradedCourse.Grade = request.Grade;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
