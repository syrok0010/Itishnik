using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;

namespace Itishnik.Application.Courses.Commands.ChangeCourseTeacher;

[Authorize(Roles = Roles.Administrator)]
public record ChangeCourseTeacherCommand(Guid CourseId, Guid NewTeacherId) : IRequest<CourseResponse>;

public class ChangeCourseTeacherCommandHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<ChangeCourseTeacherCommand, CourseResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<CourseResponse> Handle(ChangeCourseTeacherCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.Teacher)
            .FirstAsync(c => c.Id == request.CourseId, cancellationToken);
        var teacher = await _context.Teachers.FirstAsync(t => t.Id == request.NewTeacherId, cancellationToken);
        course.ChangeTeacher(teacher);
        course.Teacher.RemoveCourse(course);
        teacher.AddCourse(course);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CourseResponse>(course);
    }
}
