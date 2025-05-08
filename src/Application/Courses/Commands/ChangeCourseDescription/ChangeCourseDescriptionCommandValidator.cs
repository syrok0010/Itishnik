using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.ChangeCourseDescription;

public class ChangeCourseDescriptionCommandValidator : AbstractValidator<ChangeCourseDescriptionCommand>
{
    public ChangeCourseDescriptionCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Id)
            .MustAsync(((id, token) => context.Courses.AnyAsync(c => c.Id == id, token)))
            .WithMessage("Данного курса не существует");
    }
}
