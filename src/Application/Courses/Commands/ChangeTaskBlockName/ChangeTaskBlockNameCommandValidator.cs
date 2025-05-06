namespace Itishnik.Application.Courses.Commands.ChangeTaskBlockName;

public class ChangeTaskBlockNameCommandValidator : AbstractValidator<ChangeTaskBlockNameCommand>
{
    public ChangeTaskBlockNameCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255)
            .WithMessage("Длина названия не должна превышать 255 символов");
    }
}
