using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.SetStudentCourseGrade;

public class SetStudentCourseGradeCommandValidator : AbstractValidator<SetStudentCourseGradeCommand>
{
    public SetStudentCourseGradeCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.CourseId)
            .MustAsync((id, token) => context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курс не существует");
        RuleFor(x => x.StudentId)
            .MustAsync((id, token) => context.Students.AnyAsync(s => s.Id == id, token))
            .WithMessage("Студент не существует");
    }
}
