using Itishnik.Application.Common.Models;
using Itishnik.Application.Courses;
using Itishnik.Application.Students;
using Itishnik.Application.Students.EditSolution;
using Itishnik.Application.Students.GetCourseById;
using Itishnik.Application.Students.GetCourses;
using Itishnik.Application.Students.SendFeedback;
using Itishnik.Application.Students.StartTaskBlock;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Itishnik.Web.Endpoints;

public class Students : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetCourses, "/courses")
            .MapGet(GetCourse, "/courses/{id}")
            .MapPatch(StartTaskBlock, "/courses/{id}/start")
            .MapPatch(EditSolution, "/courses/{id}/{blockId}/{taskId}/solution")
            .MapPatch(SendFeedback, "courses/{id}/{blockId}/feedback");
    }
    
    public async Task<Ok<PaginatedList<GradedCourseResponse>>> GetCourses(ISender sender,
        [AsParameters] GetCoursesQuery query)
    {
        var response = await sender.Send(query);
        return TypedResults.Ok(response);
    }

    public async Task<Ok<StudentCourseResponse>> GetCourse(ISender sender, Guid id)
    {
        var response = await sender.Send(new GetCourseByIdQuery(id));
        return TypedResults.Ok(response);
    }

    public async Task<Ok<GradedTaskBlockDto>> StartTaskBlock(ISender sender, Guid id, Guid blockId)
    {
        var response = await sender.Send(new StartTaskBlockCommand(id, blockId));
        return TypedResults.Ok(response);
    }

    public async Task<Results<BadRequest, Ok<SolutionDto>>> EditSolution(
        ISender sender, 
        Guid id, 
        Guid blockId, 
        Guid taskId,
        EditSolutionCommand command)
    {
        if (id != command.Id || blockId != command.BlockId || taskId != command.TaskId)
            return TypedResults.BadRequest();
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }

    public async Task<Results<BadRequest, Ok<FeedbackDto>>> SendFeedback(
        ISender sender,
        Guid id,
        Guid blockId,
        SendFeedbackCommand command)
    {
        if (command.CourseId != id || command.BlockId != blockId) return TypedResults.BadRequest();
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }
}
