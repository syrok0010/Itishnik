using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Application.Tasks.Queries.GetTaskById;

public record GetTaskByIdQuery(Guid TaskId) : IRequest<TaskResponse[]>;

public class GetTaskByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetTaskByIdQuery, TaskResponse[]>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public Task<TaskResponse[]> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        return _context.Tasks
            .GetTaskChain(_context, request.TaskId)
            .ProjectTo<TaskResponse>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);
    }
}
