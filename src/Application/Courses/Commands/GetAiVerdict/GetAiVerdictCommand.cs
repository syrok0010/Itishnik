using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Models;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.GetAiVerdict;

public record GetAiVerdictCommand(Guid TaskBlockId, Guid SolutionId) : IRequest<AiVerdictResponse>;

public class GetAiVerdictCommandHandler(IApplicationDbContext context, IAiService aiService)
    : IRequestHandler<GetAiVerdictCommand, AiVerdictResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IAiService _aiService = aiService; 
    
    public async Task<AiVerdictResponse> Handle(GetAiVerdictCommand request, CancellationToken cancellationToken)
    {
        var taskBlock = await _context.TaskBlocks
            .Include(b => b.TasksEntries)
            .FirstAsync(t => t.Id == request.TaskBlockId, cancellationToken);
        var solution = await _context.Solutions
            .Include(s => s.Task)
            .FirstAsync(s => s.Id == request.SolutionId, cancellationToken);
        var maxScore = taskBlock.TasksEntries.First(e => e.TaskId == solution.TaskId).Weight;
        var response = await _aiService.EvaluateSolutionAsync(
            maxScore, 
            solution.Task.Text, 
            solution.Text, 
            solution.Task.ReferenceSolutionText);
        return response;
    }
}
