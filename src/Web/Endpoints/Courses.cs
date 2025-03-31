using Itishnik.Application.Common.Interfaces;
using Itishnik.Web.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection.Courses.Commands.CreateCourse;

namespace Itishnik.Web.Endpoints;

public class Courses : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateCourseRequest);
    }

    public async Task<IResult> CreateCourseRequest(ISender sender, CreateCourseCommand command, IUser user)
    {
        if (user.Id == null)
        {
            return TypedResults.Unauthorized();
        }
        
        var id = await sender.Send(command with {UserId = user.Id.Value});
        return TypedResults.Created($"/{nameof(Courses)}/{id}", id);
    }
}
