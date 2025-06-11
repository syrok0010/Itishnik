using Itishnik.Application.Common.Models;
using Itishnik.Application.Courses;
using Itishnik.Application.Courses.Commands.AddTaskToBlock;
using Itishnik.Application.Courses.Commands.ChangeCourseDescription;
using Itishnik.Application.Courses.Commands.ChangeCourseTeacher;
using Itishnik.Application.Courses.Commands.ChangeTaskBlockDescription;
using Itishnik.Application.Courses.Commands.ChangeTaskBlockTimeline;
using Itishnik.Application.Courses.Commands.ChangeTaskBlockName;
using Itishnik.Application.Courses.Commands.ChangeWeightsInBlock;
using Itishnik.Application.Courses.Commands.CreateCourse;
using Itishnik.Application.Courses.Commands.CreateTaskBlock;
using Itishnik.Application.Courses.Commands.DeleteTaskFromBlock;
using Itishnik.Application.Courses.Commands.EvaluateSolutionByTeacher;
using Itishnik.Application.Courses.Commands.GetAiVerdict;
using Itishnik.Application.Courses.Commands.InviteStudentsToCourse;
using Itishnik.Application.Courses.Commands.PublishTaskBlock;
using Itishnik.Application.Courses.Commands.SetStudentCourseGrade;
using Itishnik.Application.Courses.Queries.GetCourseById;
using Itishnik.Application.Courses.Queries.GetCourseList;
using Itishnik.Application.Courses.Queries.GetGradesOnCourse;
using Itishnik.Application.Courses.Queries.GetFeedbacks;
using Itishnik.Application.Courses.Queries.GetGradedTaskBlock;
using Itishnik.Application.Courses.Queries.GetStudentsOnCourse;
using Itishnik.Application.Students;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
            .MapGet(GetStudentsAndGrades, "{id}/grades")
            .MapPatch(ChangeTaskBlockDescription, "{id}/{blockId}/description")
            .MapPatch(ChangeTaskBlockTimeline, "{id}/{blockId}/timeline")
            .MapPatch(ChangeTaskBlockName, "{id}/{blockId}/name")
            .MapPatch(ChangeDescription, "{id}/description")
            .MapPost(SetStudentCourseGrade, "{id}/set-grade")
            .MapPost(AddTaskToBlock, "{id}/{blockId}/task")
            .MapDelete(DeleteTaskFromBlock, "{id}/{blockId}/task")
            .MapPatch(ChangeWeights, "{id}/{blockId}/gradeWeights")
            .MapPost(PublishBlock, "{id}/{blockId}/publish")
            .MapPatch(ChangeTeacher, "{id}/teacher")
            .MapPost(InviteStudents, "{id}/invite")
            .MapGet(GetFeedbacks, "{id}/{blockId}/feedbacks")
            .MapGet(GetGradedTaskBlock, "{id}/{blockId}/student-solution/{studentId}")
            .MapPost(GetAiVerdict, "{id}/{blockId}/verdict")
            .MapPatch(EvaluateSolution, "{id}/{blockId}/{taskId}/{solutionId}/grade");
    }

    public async Task<Ok<SolutionDto>> EvaluateSolution(
        ISender sender,
        Guid id, 
        Guid blockId, 
        Guid taskId,
        Guid solutionId, 
        [FromBody] int grade)
    {
        var response = await sender.Send(new EvaluateSolutionByTeacherCommand(id, blockId, taskId, solutionId, grade));
        return TypedResults.Ok(response);
    }
    
    public async Task<Results<BadRequest, Ok<AiVerdictResponse>>> GetAiVerdict(ISender sender, Guid id, Guid blockId, [FromBody] GetAiVerdictCommand command)
    {
        if (command.CourseId != id || command.TaskBlockId != blockId) return TypedResults.BadRequest();
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }
    
    public async Task<Created<CourseResponse>> CreateCourse(ISender sender, CreateCourseCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Courses)}/{response.Id}", response);
    }

    public async Task<Ok<PaginatedList<CourseListResponse>>> GetCoursesList(ISender sender, [AsParameters] GetCoursesListQuery query)
    {
        var response = await sender.Send(query);
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
        return response is null 
            ? TypedResults.NotFound(response)
            : TypedResults.Ok(response);
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

    public async Task<Ok<StudentGradesResponse[]>> GetStudentsAndGrades(ISender sender, Guid id)
    {
        var response = await sender.Send(new GetGradesOnCourseQuery(id));
        return TypedResults.Ok(response);
    }

    public async Task<Results<Ok<TaskBlockResponse>, BadRequest>> ChangeTaskBlockDescription(
        ISender sender, 
        Guid id,
        Guid blockId,
        ChangeTaskBlockDescriptionCommand command)
    {
        if (command.CourseId != id || command.TaskBlockId != blockId) return TypedResults.BadRequest();
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }

    public async Task<Results<Ok<TaskBlockResponse>, BadRequest, NotFound<TaskBlockResponse>>> ChangeTaskBlockTimeline(
        ISender sender, 
        Guid id,
        Guid blockId, 
        ChangeTaskBlockTimelineCommand command)
    {
        if (command.CourseId != id || command.TaskBlockId != blockId) return TypedResults.BadRequest();
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }

    public async Task<Results<Ok<CourseResponse>, BadRequest, NotFound<CourseResponse>>> ChangeDescription(
        ISender sender, 
        Guid id, 
        ChangeCourseDescriptionCommand command)
    {
        if (command.Id != id) return TypedResults.BadRequest();
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }

    public async Task<Results<Ok<TaskBlockResponse>, BadRequest>> AddTaskToBlock(
        ISender sender,
        Guid id, 
        Guid blockId, 
        [FromBody] AddTaskToBlockCommand command)
    {
        if (command.Id != id || command.BlockId != blockId) return TypedResults.BadRequest();
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }

    public async Task<Results<NoContent, BadRequest>> DeleteTaskFromBlock(
        ISender sender, 
        Guid id, 
        Guid blockId,
        [FromBody] DeleteTaskFromBlockCommand command)
    {
        if (command.Id != id || command.BlockId != blockId) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.NoContent();
    }

    public async Task<Results<Ok<TaskBlockResponse>, BadRequest>> ChangeWeights(
        ISender sender,
        Guid id, 
        Guid blockId,
        [FromBody] ChangeWeightsInBlockCommand command)
    {
        if (command.Id != id || command.BlockId != blockId) return TypedResults.BadRequest();
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }

    public async Task<Ok<TaskBlockResponse>> PublishBlock(ISender sender, Guid id, Guid blockId)
    {
        var response = await sender.Send(new PublishTaskBlockCommand(id, blockId));
        return TypedResults.Ok(response);
    }

    public async Task<Ok<CourseResponse>> ChangeTeacher(ISender sender, Guid id, ChangeCourseTeacherCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }

    public async Task<Ok<CourseStudentListResponse>> InviteStudents(ISender sender, Guid id, InviteStudentsToCourseCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }

    public async Task<Ok<List<string>>> GetFeedbacks(
        ISender sender, Guid id, Guid blockId)
    {
        var response = await sender.Send(new GetFeedbacksQuery(id, blockId));
        return TypedResults.Ok(response);
    }
    
    public async Task<Results<Ok, BadRequest>> SetStudentCourseGrade(
        ISender sender, 
        Guid id, 
        SetStudentCourseGradeCommand command)
    {
        if (command.CourseId != id) return TypedResults.BadRequest();
        await sender.Send(command);
        return TypedResults.Ok();
    }

    public async Task<Results<Ok<GradedTaskBlockDto>, BadRequest>> GetGradedTaskBlock(ISender sender, Guid id, Guid blockId, Guid studentId)
    {
        var response = await sender.Send(new GetGradedTaskBlockQuery(id, blockId, studentId));
        return TypedResults.Ok(response);
    }
}
