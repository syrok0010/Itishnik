using System.Text.Json.Serialization;
using Itishnik.Application.Common.Interfaces;
using Itishnik.Domain.Entities;

namespace Microsoft.Extensions.DependencyInjection.Courses.Commands.CreateCourse;

public record CreateCourseCommand([property: JsonIgnore] Guid UserId, string Name, string? Description = null) : IRequest<Guid>;

public class CreateCourseCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateCourseCommand, Guid>
{
    public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var teacher = await context.Teachers.FirstAsync(x => x.Id == request.UserId, cancellationToken);
        var course = new Course(teacher, request.Name, request.Description);
        await context.Courses.AddAsync(course, cancellationToken);
        teacher.AddCourse(course);
        await context.SaveChangesAsync(cancellationToken);
        return course.Id;
    }
}
