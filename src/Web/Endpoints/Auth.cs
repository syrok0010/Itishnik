using Itishnik.Application.Common.Interfaces;
using Itishnik.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace Itishnik.Web.Endpoints;

public record AuthState(Guid UserId, IEnumerable<string>? Roles = null);

public record UserInfo(Guid Id, string Name, string Surname, string? Patronymic, string? Email);

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var groupBuilder = app.MapGroup(this);
        groupBuilder
            .MapGet("", GetUserState)
            .WithName("AuthInfo")
            .RequireAuthorization();
        groupBuilder
            .MapGet("user", GetCurrentUser)
            .WithName("UserInfo")
            .RequireAuthorization();
    }

    private static Results<Ok<AuthState>, BadRequest> GetUserState(IUser user)
    {
        return TypedResults.Ok(new AuthState(user.Id ?? Guid.Empty, user.Roles));
    }

    private static async Task<Results<Ok<UserInfo>, BadRequest>> GetCurrentUser(UserManager<ApplicationUser> userManager, IUser user)
    {
        var currentUser = await userManager.FindByIdAsync(user.Id.ToString()!);
        return currentUser is null
            ? TypedResults.BadRequest()
            : TypedResults.Ok(new UserInfo(currentUser.Id, currentUser.Name, currentUser.Surname,
                currentUser.Patronymic, currentUser.Email));
    }
}
