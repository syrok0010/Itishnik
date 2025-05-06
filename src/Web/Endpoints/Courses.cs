using Itishnik.Application.Common.Models;
using Itishnik.Application.Courses;
using Itishnik.Application.Courses.Commands.ChangeTaskBlockName;
using Itishnik.Application.Courses.Commands.CreateCourse;
using Itishnik.Application.Courses.Commands.CreateTaskBlock;
using Itishnik.Application.Courses.Queries.GetCourseById;
using Itishnik.Application.Courses.Queries.GetCourseList;
using Itishnik.Application.Courses.Queries.GetStudentsOnCourse;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Itishnik.Web.Endpoints;

public class Courses : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateCourse)
            .MapGet(GetCoursesList)
            .MapGet(GetCourseById, "{id}")
            .MapPost(CreateTaskBlock, "{id}/block")
            .MapGet(GetStudents, "{id}/students")
            .MapPatch("{id:guid}/{blockId:guid}/name", ChangeTaskBlockName);
    }
    
    public async Task<Created<CourseResponse>> CreateCourse(ISender sender, CreateCourseCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Courses)}/{response.Id}", response);
    }

    public async Task<
        Results<
            Ok<PaginatedList<CourseListResponse>>,
            NotFound<PaginatedList<CourseListResponse>>
        >
    > GetCoursesList(ISender sender, [AsParameters] GetCoursesListQuery query)
    {
        var response = await sender.Send(query);

        if (response.TotalCount == 0)
        {
            return TypedResults.NotFound(response);
        }
        return TypedResults.Ok(response);
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

    public async Task<Results<Created<TaskBlockResponse>, BadRequest>> CreateTaskBlock(ISender sender, Guid id, CreateTaskBlockCommand command)
    {
        if (command.CourseId != id) return TypedResults.BadRequest();
        var response = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Courses)}/{response.Id}", response);
    }

    public async Task<Results<Ok<TaskBlockResponse>, BadRequest, NotFound<TaskBlockResponse>>> ChangeTaskBlockName(
        ISender sender, 
        Guid id, 
        Guid blockId,
        ChangeTaskBlockNameCommand command)
    {
        if (command.CourseId != id || command.TaskBlockId != blockId) return TypedResults.BadRequest();
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }

    public async Task<Ok<CourseStudentListResponse>> GetStudents(ISender sender, Guid id)
    {
        var response = await sender.Send(new GetStudentsOnCourseQuery(id));
        return TypedResults.Ok(response);
    }
}
