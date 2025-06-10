using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Courses.Commands.GetAiVerdict;

public class GetAiVerdictCommandValidator : AbstractValidator<GetAiVerdictCommand>
{
    public GetAiVerdictCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.CourseId)
            .MustAsync((id, token) => context.Courses.AnyAsync(c => c.Id == id, token))
            .WithMessage("Курс не существует");
        RuleFor(x => x.TaskBlockId)
            .MustAsync((id, token) => context.TaskBlocks.AnyAsync(c => c.Id == id, token))
            .WithMessage("Работа не существует не существует");
        RuleFor(x => x.SolutionId)
            .MustAsync((id, token) => context.Solutions.AnyAsync(c => c.Id == id, token))
            .WithMessage("Решение не существует");
        RuleFor(x => x)
            .MustAsync((cmd, token) => RelationsCheck(context, cmd, token))
            .WithMessage("Нарушение связи между курсом, заданием и решением");

    }

    private async Task<bool> RelationsCheck(IApplicationDbContext context, GetAiVerdictCommand command,
        CancellationToken token)
    {
        var course = await context.Courses
            .Include(c => c.TaskBlocks)
            .FirstAsync(c => c.Id == command.CourseId, token);
        
        if (course.TaskBlocks.All(t => t.Id != command.TaskBlockId))
        {
            return false;
        }

        var solution = await context.Solutions
            .Include(s => s.Task).ThenInclude(t => t.TaskBlockEntries)
            .FirstAsync(s => s.Id == command.SolutionId, token);

        return solution.Task.TaskBlockEntries.Any(tbe => tbe.TaskBlockId == command.TaskBlockId);
    }
}
