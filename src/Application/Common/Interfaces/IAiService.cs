using Itishnik.Application.Common.Models;

namespace Itishnik.Application.Common.Interfaces;

public interface IAiService
{
    Task<AiVerdictResponse> EvaluateSolutionAsync(
        int maxScore,
        string taskText,
        string solutionText,
        string referenceText);
    
    Task<AiGeneratedTaskResponse> GenerateTaskAsync(
        string topic, 
        string difficulty,
        string taskType, 
        string? info = null,
        string? theme = null);
}
