using Itishnik.Application.Common.Models;
using Itishnik.Application.Tasks;
using Itishnik.Application.Tasks.Commands.CreateTag;
using Itishnik.Application.Tasks.Commands.CreateTask;
using Itishnik.Application.Tasks.Commands.EditReferenceSolution;
using Itishnik.Application.Tasks.Commands.PublishTask;
using Itishnik.Application.Tasks.Commands.SetTaskTags;
using Itishnik.Application.Tasks.Queries.GetTagList;
using Itishnik.Application.Tasks.Queries.GetTaskById;
using Itishnik.Application.Tasks.Queries.GetTaskList;
using Itishnik.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Itishnik.Web.Endpoints;

public class Tasks : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateTask)
            .MapPost(CreateTag, "tags")
            .MapGet(GetTagList, "tags")
            .MapGet(GetTaskList)
            .MapGet(GetTaskWithAllVersions, "{id}")
            .MapPatch(SetTaskTags, "{id}/tags")
            .MapPatch(Publish, "{id}/publish")
            .MapPatch(EditReferenceSolution, "{id}/solution");
    }
    
    public async Task<Created<TaskResponse[]>> CreateTask(ISender sender, CreateTaskCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Tasks)}/{response.Last().Id}", response);
    }
    
    public async Task<Created<Tag>> CreateTag(ISender sender, CreateTagCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Tasks)}/tags", response);
    }
    
    public async Task<Results<Ok<List<Tag>>, NotFound<List<Tag>>>> GetTagList(ISender sender)
    {
        var response = await sender.Send(new GetTagListQuery());
        return response.Count == 0 ? TypedResults.NotFound(response) : TypedResults.Ok(response);
    }
    
    public async Task<PaginatedList<TaskListResponse>> GetTaskList(ISender sender, [AsParameters] GetTaskListQuery query)
    {
        return await sender.Send(query);
    }
    
    public async Task<Ok<TaskResponse[]>> GetTaskWithAllVersions(ISender sender, Guid id)
    {
        var response = await sender.Send(new GetTaskByIdQuery(id));
        return TypedResults.Ok(response);
    }
    
    public async Task<Results<Ok<TaskResponse[]>, BadRequest>> SetTaskTags(ISender sender, Guid id, SetTaskTagsCommand command)
    {
        if (id != command.TaskId)
            return TypedResults.BadRequest();
        
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }
    
    public async Task<Results<Ok<TaskResponse[]>, BadRequest>> Publish(ISender sender, Guid id)
    {
        var response = await sender.Send(new PublishTaskCommand(id));
        return TypedResults.Ok(response);
    }

    public async Task<Results<Ok<TaskResponse>, BadRequest>> EditReferenceSolution(ISender sender, Guid id,
        EditReferenceSolutionCommand command)
    {
        if (id != command.TaskId) return TypedResults.BadRequest();
        var response = await sender.Send(command);
        return TypedResults.Ok(response);
    }
}
