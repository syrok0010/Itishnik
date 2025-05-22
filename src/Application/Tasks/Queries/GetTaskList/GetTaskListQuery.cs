using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Mappings;
using Itishnik.Application.Common.Models;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;

namespace Itishnik.Application.Tasks.Queries.GetTaskList;

[Authorize(Roles = Roles.Teacher)]
[Authorize(Roles = Roles.Administrator)]
public record GetTaskListQuery(
    Guid[]? ThemeIds,
    Guid[]? AuthorIds,
    string? Name,
    string? SortBy,
    bool Ascending = true,
    int PageNumber = 1,
    int PageSize = 25) : IRequest<PaginatedList<TaskListResponse>>;

public class GetTaskListQueryHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<GetTaskListQuery, PaginatedList<TaskListResponse>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public Task<PaginatedList<TaskListResponse>> Handle(GetTaskListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tasks
            .AsNoTracking()
            .AsQueryable();

        // Только последние версии
        query = query
            .Where(t => !_context.Tasks.Any(otherTask => otherTask.PreviousVersion != null && otherTask.PreviousVersion.Id == t.Id));

        if (request.ThemeIds is not null && request.ThemeIds.Length > 0) 
            query = query.Where(task => task.Tags.Any(tag => request.ThemeIds.Contains(tag.Id)));

        if (request.AuthorIds is not null && request.AuthorIds.Length > 0) 
            query = query.Where(task => request.AuthorIds.Contains(task.TeacherId));

        if (!string.IsNullOrWhiteSpace(request.Name)) 
            query = query.Where(task => task.Name.ToLower().Contains(request.Name.ToLower()));

        query = (request.SortBy, request.Ascending) switch
        {
            ("name", true) => query.OrderBy(task => task.Name),
            ("name", false) => query.OrderByDescending(task => task.Name),
            ("author", true) => query.OrderBy(task => task.Teacher.Surname).ThenBy(x => x.Teacher.Name).ThenBy(x => x.Teacher.Patronymic),
            ("author", false) => query.OrderByDescending(task => task.Teacher.Surname).ThenByDescending(x => x.Teacher.Name).ThenByDescending(x => x.Teacher.Patronymic),
            ("isPublic", true) => query.OrderBy(task => task.IsPublic),
            ("isPublic", false) => query.OrderByDescending(task => task.IsPublic),
            _ => query
        };

        return query
            .ProjectTo<TaskListResponse>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
