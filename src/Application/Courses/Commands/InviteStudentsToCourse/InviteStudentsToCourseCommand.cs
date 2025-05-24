using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace Itishnik.Application.Courses.Commands.InviteStudentsToCourse;

[Authorize(Policy = Policies.OwnerOrAdmin)]
[ResourceMetadata(nameof(Id), typeof(Course))]
public record InviteStudentsToCourseCommand(Guid Id, ICollection<string> Emails) : IRequest;

public class InviteStudentsToCourseCommandHandler(IApplicationDbContext context, IIdentityService service) 
    : IRequestHandler<InviteStudentsToCourseCommand>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IIdentityService _service = service; 
    
    public async Task Handle(InviteStudentsToCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.GradedCourses)
            .FirstAsync(c => c.Id == request.Id, cancellationToken);

        var existingStudents = await _context.Students
            .Where(s => request.Emails.Contains(s.Email!))
            .ToListAsync(cancellationToken);
        var newUsers = request.Emails.Where(e => !existingStudents.Select(s => s.Email).Contains(e));
        foreach (var newUser in newUsers)
        {
            var result = await _service.CreateUserAsync<Student>(newUser);
            if (!result.Result.Succeeded)
            {
                continue;
            }

            var token = await _service.GetPasswordResetTokenAsync(result.User.Id);
            
        }
    }
}
