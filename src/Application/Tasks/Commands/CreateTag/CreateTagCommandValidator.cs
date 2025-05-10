using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Tasks.Commands.CreateTag;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator(IApplicationDbContext db)
    {
        RuleFor(x => x.Text)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Тег не может быть пустым")
            .MustAsync((tag, ct) => db.Tags.AllAsync(t => t.Text != tag, ct))
            .WithMessage("Такой тег уже существует");
    }
}
