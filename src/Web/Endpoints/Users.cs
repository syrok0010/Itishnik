using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Users.Queries.GetUserList;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Itishnik.Web.Endpoints;

public record AuthState(Guid UserId, IEnumerable<string>? Roles = null);

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var groupBuilder = app.MapGroup(this);
        groupBuilder
            .MapGet("/auth-info", GetAuthState)
            .WithName("AuthInfo")
            .RequireAuthorization();

        groupBuilder.MapGet(GetUsers);
    }

    private static AuthState GetAuthState(IUser user) => new(user.Id ?? Guid.Empty, user.Roles);

    private static async Task<Results<Ok<IEnumerable<UserDto>>, BadRequest>> GetUsers(ISender sender, [AsParameters] GetUserListQuery query)
    {
        return TypedResults.Ok(await sender.Send(query));
    }
}
