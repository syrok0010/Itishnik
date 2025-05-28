using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.InviteStudentsToCourse;

[Authorize(Policy = Policies.OwnerOrAdmin)]
[ResourceMetadata(nameof(Id), typeof(Course))]
public record InviteStudentsToCourseCommand(Guid Id, ICollection<string> Emails) : IRequest<CourseStudentListResponse>;

public class InviteStudentsToCourseCommandHandler(
    IApplicationDbContext db,
    IIdentityService identityService,
    IResetPasswordService resetService,
    IMapper mapper)
    : IRequestHandler<InviteStudentsToCourseCommand, CourseStudentListResponse>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IIdentityService _identityService = identityService;
    private readonly IResetPasswordService _resetService = resetService;
    private readonly IMapper _mapper = mapper;

    public async Task<CourseStudentListResponse> Handle(InviteStudentsToCourseCommand request, CancellationToken cancellationToken)
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
            var result = await _identityService.CreateUserAsync(student);
            if (!result.Succeeded)
                continue;

            await _resetService.SendResetPasswordEmail(newUserEmail);
            existingStudents.Add(student);
        }

        await _db.GradedCourses.AddRangeAsync(
            existingStudents.Select(s => new GradedCourse(course, s)), cancellationToken
        );
        await _db.SaveChangesAsync(cancellationToken);
        return new CourseStudentListResponse { Students = _mapper.Map<List<StudentDto>>(existingStudents) };
    }
}
