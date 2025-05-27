using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace Itishnik.Application.Users.Commands.ActivateStudent;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(StudentId), typeof(Student))]
public record ActivateStudentCommand(
    Guid StudentId,
    string Name,
    string Surname,
    string? Patronymic,
    int EducationStartYear,
    int GroupNumber,
    string EducationalProgram) : IRequest;

public class ActivateStudentCommandHandler(IApplicationDbContext db) : IRequestHandler<ActivateStudentCommand>
{
    private readonly IApplicationDbContext _db = db;

    public async Task Handle(ActivateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _db.Students.FirstAsync(s => s.Id == request.StudentId, cancellationToken);
        student.Name = request.Name;
        student.Surname = request.Surname;
        student.Patronymic = request.Patronymic;
        student.EducationStartYear = request.EducationStartYear;
        student.GroupNumber = request.GroupNumber;
        student.EducationalProgram = request.EducationalProgram;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
