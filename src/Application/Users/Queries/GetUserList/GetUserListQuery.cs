using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;
using Itishnik.Domain.Constants;

namespace Itishnik.Application.Users.Queries.GetUserList;

[Authorize(Roles = Domain.Constants.Roles.Administrator)]
[Authorize(Roles = Domain.Constants.Roles.Teacher)]
public record GetUserListQuery(string[] Roles) : IRequest<IEnumerable<UserDto>>;

public sealed class GetUserListQueryHandler(IApplicationDbContext db, IMapper mapper) : IRequestHandler<GetUserListQuery, IEnumerable<UserDto>>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public Task<IEnumerable<UserDto>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<UserDto> result = [];
        
        if (request.Roles.Contains(Roles.Student)) 
            result = result.Union(_db.Students.AsNoTracking().ProjectTo<UserDto>(_mapper.ConfigurationProvider));
        
        if (request.Roles.Contains(Roles.Teacher)) 
            result = result.Union(_db.Teachers.AsNoTracking().ProjectTo<UserDto>(_mapper.ConfigurationProvider));
     
        return Task.FromResult(result);
    }
}
