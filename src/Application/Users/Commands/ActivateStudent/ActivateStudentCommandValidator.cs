using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Users.Commands.ActivateStudent;

public class ActivateStudentCommandValidator : AbstractValidator<ActivateStudentCommand>
{
    public ActivateStudentCommandValidator(IApplicationDbContext db)
    {
        RuleFor(x => x.StudentId)
            .MustAsync((studentId, token) =>
                db.Students.AnyAsync(s => s.Id == studentId && s.GroupNumber == 100, token))
            .WithMessage("Студент с таким Id не существует или уже активирован.");
        
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.Surname)
            .NotEmpty();
        
        RuleFor(x => x.GroupNumber)
            .GreaterThan(0);
        RuleFor(x => x.EducationStartYear)
            .GreaterThanOrEqualTo(TimeProvider.System.GetLocalNow().Year - 5)
            .LessThanOrEqualTo(TimeProvider.System.GetLocalNow().Year + 5);
        RuleFor(x => x.EducationalProgram)
            .NotEmpty();
    }
}
