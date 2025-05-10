using Itishnik.Application.Common.Interfaces;
using Itishnik.Domain.Entities;

namespace Itishnik.Application.Tasks.Queries.GetTagList;

public record GetTagListQuery : IRequest<List<Tag>>;

public class GetTagListQueryHandler(IApplicationDbContext db) : IRequestHandler<GetTagListQuery, List<Tag>>
{
    public Task<List<Tag>> Handle(GetTagListQuery request, CancellationToken cancellationToken) 
        => db.Tags.ToListAsync(cancellationToken);
}
