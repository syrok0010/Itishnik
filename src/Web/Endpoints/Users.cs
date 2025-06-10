using AutoMapper;
using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Users.Commands.ActivateStudent;
using Itishnik.Application.Users.Commands.ActivateTeacher;
using Itishnik.Application.Users.Commands.InviteTeachers;
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
        
        groupBuilder
            .MapGet(GetUsers)
            .MapPost(ActivateStudent, "activate-student")
            .MapPost(ActivateTeacher, "activate-teacher")
            .MapPost(InviteTeachers, "invite-teachers");
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

    private static async Task<Results<Ok, BadRequest>> ActivateStudent(ISender sender, ActivateStudentCommand command)
    {
        await sender.Send(command);
        return TypedResults.Ok();
    }
    
    private static async Task<Results<Ok, BadRequest>> ActivateTeacher(ISender sender, ActivateTeacherCommand command)
    {
        await sender.Send(command);
        return TypedResults.Ok();
    }

    private static async Task<Ok<UserDto[]>> InviteTeachers(ISender sender, InviteTeachersCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }
}
