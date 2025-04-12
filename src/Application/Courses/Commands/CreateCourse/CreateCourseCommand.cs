using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.CreateCourse;

[Authorize(Roles = "Teacher")]
public record CreateCourseCommand(string Name, string? Description = null) : IRequest<CourseResponse>;

public class CreateCourseCommandHandler(IApplicationDbContext context, IUser user, IMapper mapper) 
    : IRequestHandler<CreateCourseCommand, CourseResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;
    private readonly IMapper _mapper = mapper;

    public async Task<CourseResponse> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _context.Teachers.FirstAsync(x => x.Id == _user.Id, cancellationToken);
        var course = new Course(teacher, request.Name, request.Description);
        await _context.Courses.AddAsync(course, cancellationToken);
        teacher.AddCourse(course);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CourseResponse>(course);
    }
}
