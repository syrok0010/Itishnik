using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Models;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;

namespace Itishnik.Application.Tasks.Commands.GenerateTask;

[Authorize(Roles = Roles.Teacher)]
public record GenerateTaskCommand(
    string Topic,
    string Difficulty, 
    string TaskType, 
    string? Idea = null,
    string? Theme = null)
    : IRequest<AiGeneratedTaskResponse>;

public class GenerateTaskCommandHandler(IAiService aiService, IApplicationDbContext context)
    : IRequestHandler<GenerateTaskCommand, AiGeneratedTaskResponse>
{
    private readonly IAiService _aiService = aiService;
    private readonly IApplicationDbContext _context = context;
    
    public async Task<AiGeneratedTaskResponse> Handle(GenerateTaskCommand request, CancellationToken cancellationToken)
    {
        return await _aiService.GenerateTaskAsync(
            request.Topic,
            request.Difficulty,
            request.TaskType,
            request.Idea,
            request.Theme);
    }
}
