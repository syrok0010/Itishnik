using Itishnik.Application.Common.Interfaces;
using Itishnik.Domain.Entities;

namespace Microsoft.Extensions.DependencyInjection.Courses.Commands.CreateCourse;

public record CreateCourseCommand(string Name, string Description, string TeacherId) : IRequest<Guid>;

public class CreateCourseCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateCourseCommand, Guid>
{
    public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var teacher = await context.Teachers.FirstAsync(x => x.Id == request.TeacherId, cancellationToken);
        var course = new Course(request.Name, request.Description, teacher);
        await context.Courses.AddAsync(course, cancellationToken);
        teacher.AddCourse(course);
        await context.SaveChangesAsync(cancellationToken);
        return course.Id;
    }
}
