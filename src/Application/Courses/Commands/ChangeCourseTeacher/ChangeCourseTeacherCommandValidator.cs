using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.ChangeCourseTeacher;

public class ChangeCourseTeacherCommandValidator : AbstractValidator<ChangeCourseTeacherCommand>
{
    public ChangeCourseTeacherCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.CourseId)
            .MustAsync((id, token) => context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курс не существует");
        RuleFor(x => x.NewTeacherId)
            .MustAsync((id, token) => context.Teachers.AnyAsync(t => t.Id == id, token))
            .WithMessage("Учитель не существует");
    }
}
