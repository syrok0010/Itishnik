using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Mappings;
using Itishnik.Application.Common.Models;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;

namespace Itishnik.Application.Tasks.Queries.GetTaskList;

[Authorize(Roles = Roles.Teacher)]
[Authorize(Roles = Roles.Administrator)]
public record GetTaskListQuery(int PageNumber = 1, int PageSize = 25) : IRequest<PaginatedList<TaskListResponse>>;

public class GetTaskListQueryHandler(IApplicationDbContext context, IMapper mapper, IUser currentUser) 
    : IRequestHandler<GetTaskListQuery, PaginatedList<TaskListResponse>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _currentUser = currentUser;

    public Task<PaginatedList<TaskListResponse>> Handle(GetTaskListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tasks.AsQueryable();

        if (_currentUser.Roles.Contains(Roles.Teacher)) 
            query = query.Where(t => t.IsPublic || t.TeacherId == _currentUser.Id);
        
        query = query
            .Where(t => !_context.Tasks.Any(otherTask => otherTask.PreviousVersion != null && otherTask.PreviousVersion.Id == t.Id));
        
        return query
            .ProjectTo<TaskListResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
