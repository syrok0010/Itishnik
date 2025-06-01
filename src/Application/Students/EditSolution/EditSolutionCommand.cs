using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Students.EditSolution;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(Id), typeof(GradedCourse))]
public record EditSolutionCommand(Guid Id, Guid BlockId, Guid TaskId, string Text) 
    : IRequest<SolutionDto>;

public class EditSolutionCommandHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<EditSolutionCommand, SolutionDto>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<SolutionDto> Handle(EditSolutionCommand request, CancellationToken cancellationToken)
    {
        var solution = await _context.Solutions.FirstAsync(s => s.TaskId == request.TaskId, cancellationToken);
        solution.Text = request.Text;
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<SolutionDto>(solution);
    }
}

