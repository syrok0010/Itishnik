using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Students.SendFeedback;

public class SendFeedbackCommandValidator : AbstractValidator<SendFeedbackCommand>
{
    public SendFeedbackCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.CourseId)
            .MustAsync((id, token) => context.GradedCourses.AnyAsync(gc => gc.Id == id, token))
            .WithMessage("Курс не существует");
        RuleFor(x => x.BlockId)
            .MustAsync((id, token) => context.GradedTaskBlocks.AnyAsync(gtb => gtb.Id == id, token))
            .WithMessage("Работа не существует");
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Текст комментария пуст");
    }
}
