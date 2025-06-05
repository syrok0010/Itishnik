using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Students.StartTaskBlock;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(Id), typeof(GradedCourse))]
public record StartTaskBlockCommand(Guid Id, Guid BlockId) : IRequest<GradedTaskBlockDto>;

public class StartTaskBlockCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<StartTaskBlockCommand, GradedTaskBlockDto>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    
    public async Task<GradedTaskBlockDto> Handle(StartTaskBlockCommand request, CancellationToken cancellationToken)
    {
        var block = await _context.GradedTaskBlocks
            .Include(gtb => gtb.TaskBlock).ThenInclude(tb => tb.Tasks)
            .Include(gtb => gtb.Solutions).ThenInclude(s => s.Task)
            .FirstAsync(gtb => gtb.Id == request.BlockId, cancellationToken);
        block.Start();
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<GradedTaskBlockDto>(block);
    }
}
