using AutoMapper;
using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Users.Queries.GetUserList;
using Itishnik.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

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

        groupBuilder
            .MapGet("user", GetCurrentUser)
            .WithName("UserInfo")
            .RequireAuthorization();
        
        groupBuilder.MapGet(GetUsers);
    }

    private static AuthState GetAuthState(IUser user) => new(user.Id ?? Guid.Empty, user.Roles);

    private static async Task<Results<Ok<IEnumerable<UserDto>>, BadRequest>> GetUsers(ISender sender, [AsParameters] GetUserListQuery query)
    {
        return TypedResults.Ok(await sender.Send(query));
    }
    
    private static async Task<Results<Ok<UserDto>, BadRequest>> GetCurrentUser(UserManager<ApplicationUser> userManager, IMapper mapper, IUser user)
    {
        var currentUser = await userManager.FindByIdAsync(user.Id.ToString()!);
        return currentUser is null
            ? TypedResults.BadRequest()
            : TypedResults.Ok(mapper.Map<UserDto>(currentUser));
    }
}
