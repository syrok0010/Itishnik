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
    string? Info = null,
    string? Theme = null)
    : IRequest<AiGeneratedTaskResponse>;

public class GenerateTaskCommandHandler(IAiService aiService)
    : IRequestHandler<GenerateTaskCommand, AiGeneratedTaskResponse>
{
    private readonly IAiService _aiService = aiService;
    
    public async Task<AiGeneratedTaskResponse> Handle(GenerateTaskCommand request, CancellationToken cancellationToken)
    {
        return await _aiService.GenerateTaskAsync(
            request.Topic, 
            request.Difficulty, 
            request.TaskType, 
            request.Info, 
            request.Theme);
    }
}
