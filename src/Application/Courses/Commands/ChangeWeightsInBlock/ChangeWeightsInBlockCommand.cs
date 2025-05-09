using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Courses.Commands.ChangeWeightsInBlock;


[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(Id), typeof(Course))]
public record ChangeWeightsInBlockCommand(Guid Id, Guid BlockId, IEnumerable<int> Weights) : IRequest<TaskBlockResponse>;

public class ChangeWeightsInBlockCommandHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<ChangeWeightsInBlockCommand, TaskBlockResponse>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<TaskBlockResponse> Handle(ChangeWeightsInBlockCommand request, CancellationToken cancellationToken)
    {
        var taskBlock = await _context.TaskBlocks
            .Include(tb => tb.Tasks)
            .FirstAsync(tb => tb.Id == request.BlockId, cancellationToken);

        foreach ((int weight, int taskNumber) in request.Weights.Zip(Enumerable.Range(1, request.Weights.Count())))
        {
            taskBlock.ChangeWeight(taskNumber, weight);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<TaskBlockResponse>(taskBlock);
    }
}
