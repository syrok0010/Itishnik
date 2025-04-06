using Itishnik.Application.Courses;
using Itishnik.Application.Courses.Commands.CreateCourse;
using Itishnik.Application.Courses.Queries;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Itishnik.Web.Endpoints;

public class Courses : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateCourseRequest)
            .MapGet(GetCourseById, "{id}");
    }
    
    public async Task<Created<CourseResponse>> CreateCourseRequest(ISender sender, CreateCourseCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Courses)}/{response.Id}", response);
    }

    public async Task<
        Results<
            Ok<CourseResponse>,
            NotFound<CourseResponse>,
            BadRequest
        >
    > GetCourseById(ISender sender, Guid id) 
    {
        var response = await sender.Send(new GetCourseByIdQuery(id));
        if (response == null)
        {
            return TypedResults.NotFound(response);
        }

        return TypedResults.Ok(response);
    }
}
