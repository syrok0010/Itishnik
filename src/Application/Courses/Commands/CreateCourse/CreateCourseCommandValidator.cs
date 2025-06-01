using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.CreateCourse;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator(IApplicationDbContext db)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255)
            .WithMessage("Длина названия не должна превышать 255 символов")
            .MustAsync((name, token) => db.Courses.AllAsync(c => c.Name != name, token))
            .WithMessage("Курс с таким названием уже существует");
    }
}
