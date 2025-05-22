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
            .Cascade(CascadeMode.Stop)
            .MustAsync((id, token) => context.Teachers.AnyAsync(t => t.Id == id, token))
            .WithMessage("Учитель не существует")
            .MustAsync((cmd, id, token) => TeacherCheck(context, cmd, token))
            .WithMessage("Учитель уже является владельцем курса");
    }

    private async Task<bool> TeacherCheck(IApplicationDbContext context, ChangeCourseTeacherCommand command, CancellationToken token)
    {
        var course = await context.Courses
            .Include(c => c.Teacher)
            .FirstAsync(c => c.Id == command.CourseId, token);
        return course.TeacherId != command.NewTeacherId;
    }
}
