using Itishnik.Application.Courses;
using Itishnik.Application.Courses.Commands.CreateCourse;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Itishnik.Web.Endpoints;

public class Courses : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateCourseRequest);
    }
    
    public async Task<Created<CourseResponse>> CreateCourseRequest(ISender sender, CreateCourseCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Courses)}/{response.Id}", response);
    }
}
