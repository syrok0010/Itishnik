using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Models;

namespace Itishnik.Application.Tasks.Commands.GenerateTask;

public record GenerateTaskCommand(Guid TaskId, string? AdditionalInformation = null)
    : IRequest<AiGeneratedTaskResponse>;

public class GenerateTaskCommandHandler(IAiService aiService, IApplicationDbContext context)
    : IRequestHandler<GenerateTaskCommand, AiGeneratedTaskResponse>
{
    private readonly IAiService _aiService = aiService;
    private readonly IApplicationDbContext _context = context;
    
    public async Task<AiGeneratedTaskResponse> Handle(GenerateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Tags)
            .FirstAsync(t => t.Id == request.TaskId, cancellationToken);
        var tags = task.Tags.Select(t => t.Text).ToList();
        return await _aiService.GenerateTaskAsync(task.Text, tags, request.AdditionalInformation ?? "Не представлено");
    }
}
