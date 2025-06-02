using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Students.SendFeedback;

[Authorize(Policy = Policies.Owner)]
[ResourceMetadata(nameof(CourseId), typeof(GradedCourse))]
public record SendFeedbackCommand(Guid CourseId, Guid BlockId, string Text) : IRequest<FeedbackDto>;

public class SendFeedbackCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<SendFeedbackCommand, FeedbackDto>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<FeedbackDto> Handle(SendFeedbackCommand request, CancellationToken cancellationToken)
    {
        var block = await _context.GradedTaskBlocks.FirstAsync(gc => gc.Id == request.BlockId, cancellationToken);
        block.Feedback = request.Text;
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<FeedbackDto>(block);
    }
}
