using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace Itishnik.Application.Courses.Commands.InviteStudentsToCourse;

[Authorize(Policy = Policies.OwnerOrAdmin)]
[ResourceMetadata(nameof(Id), typeof(Course))]
public record InviteStudentsToCourseCommand(Guid Id, ICollection<string> Emails) : IRequest;

public class InviteStudentsToCourseCommandHandler(
    IApplicationDbContext db,
    IIdentityService service,
    IResetPasswordService resetService)
    : IRequestHandler<InviteStudentsToCourseCommand>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IIdentityService _service = service;
    private readonly IResetPasswordService _resetService = resetService;

    public async Task Handle(InviteStudentsToCourseCommand request, CancellationToken cancellationToken)
    {
        const string notSet = "Не установлено";
        var course = await _db.Courses
            .Include(c => c.GradedCourses)
            .FirstAsync(c => c.Id == request.Id, cancellationToken);

        var existingStudents = await _db.Students
            .Where(s => request.Emails.Contains(s.Email!))
            .ToListAsync(cancellationToken);
        var newUsers = request.Emails.Where(e => !existingStudents.Select(s => s.Email).Contains(e));
        foreach (var newUserEmail in newUsers)
        {
            var student = new Student(
                notSet,
                notSet,
                null,
                notSet,
                100,
                TimeProvider.System.GetLocalNow().Year
            ) { Email = newUserEmail, UserName = newUserEmail, EmailConfirmed = true };
            var result = await _service.CreateUserAsync(student);
            if (!result.Succeeded)
                continue;

            await _resetService.SendResetPasswordEmail(newUserEmail);
            existingStudents.Add(student);
        }

        await _db.GradedCourses.AddRangeAsync(
            existingStudents.Select(s => new GradedCourse(course, s)), cancellationToken
        );
        await _db.SaveChangesAsync(cancellationToken);
    }
}
