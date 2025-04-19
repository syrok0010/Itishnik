using Itishnik.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Itishnik.Web.Endpoints;

public record AuthState(Guid UserId, IEnumerable<string>? Roles = null);

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet("", GetUserState)
            .WithName("AuthInfo")
            .RequireAuthorization();
    }

    private static Results<Ok<AuthState>, BadRequest> GetUserState(IUser user)
    {
        return TypedResults.Ok(new AuthState(user.Id ?? Guid.Empty, user.Roles));
    }
}
