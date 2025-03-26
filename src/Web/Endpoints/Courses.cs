using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection.Courses.Commands.CreateCourse;

namespace Itishnik.Web.Endpoints;

public class Courses : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateCourse);
    }

    public async Task<Created<Guid>> CreateCourse(ISender sender, CreateCourseCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Courses)}/{id}", id);
    }
}
