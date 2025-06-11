using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Application.Students;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.EvaluateSolutionByTeacher;

[Authorize(Policy = Policies.OwnerOrAdmin)]
[ResourceMetadata(nameof(CourseId), typeof(Course))]
public record EvaluateSolutionByTeacherCommand(
    Guid CourseId,
    Guid TaskBlockId,
    Guid TaskId,
    Guid SolutionId,
    int Grade) : IRequest<SolutionDto>;

public class EvaluateSolutionByTeacherCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<EvaluateSolutionByTeacherCommand, SolutionDto>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<SolutionDto> Handle(EvaluateSolutionByTeacherCommand request, CancellationToken cancellationToken)
    {
        var solution = await _context.Solutions
            .Include(s => s.Task).ThenInclude(t => t.TaskBlockEntries)
            .FirstAsync(s => s.Id == request.SolutionId, cancellationToken);
        solution.Grade = request.Grade;
        await _context.SaveChangesAsync(cancellationToken);
        var mapped = _mapper.Map<SolutionDto>(solution);
        var entry = solution.Task.TaskBlockEntries.First(tbe => tbe.TaskBlockId == request.TaskBlockId);
        mapped.Weight = entry.Weight;
        mapped.Position = entry.Position;
        return mapped;
    }
}
