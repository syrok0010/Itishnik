using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Users.Commands.ActivateTeacher;

public class ActivateTeacherCommandValidator : AbstractValidator<ActivateTeacherCommand>
{
    public ActivateTeacherCommandValidator(IApplicationDbContext db)
    {
        RuleFor(x => x.Name)
            .NotEmpty();
        RuleFor(x => x.Surname)
            .NotEmpty();
    }
}
