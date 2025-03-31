using Itishnik.Application.Common.Interfaces;

namespace Microsoft.Extensions.DependencyInjection.Courses.Commands.CreateCourse;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator(IApplicationDbContext db)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255)
            .WithMessage("Длина названия не должна превышать 255 символов");
    }
}
