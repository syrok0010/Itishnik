using Itishnik.Application.Common.Models;
using Itishnik.Application.Courses;
using Itishnik.Application.Students;
using Itishnik.Application.Students.GetCourses;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Itishnik.Web.Endpoints;

public class Students : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetCourses, "/courses");
    }
    
    public async Task<Ok<PaginatedList<GradedCourseResponse>>> GetCourses(ISender sender,
        [AsParameters] GetCoursesQuery query)
    {
        var response = await sender.Send(query);
        return TypedResults.Ok(response);
    }
}
