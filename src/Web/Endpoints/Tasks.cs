using Itishnik.Application.Common.Models;
using Itishnik.Application.Tasks;
using Itishnik.Application.Tasks.Commands.CreateTag;
using Itishnik.Application.Tasks.Commands.CreateTask;
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
            .MapPost(CreateTaskRequest)
            .MapPost(CreateTag, "tags")
            .MapGet(GetTagList, "tags")
            .MapGet(GetTaskList)
            .MapGet(GetTaskWithAllVersions, "{id}");
    }
    
    public async Task<Created<TaskResponse[]>> CreateTaskRequest(ISender sender, CreateTaskCommand command)
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
    
    public async Task<
        Results<
            Ok<PaginatedList<TaskListResponse>>,
            NotFound<PaginatedList<TaskListResponse>>
        >
    > GetTaskList(ISender sender, [AsParameters] GetTaskListQuery query)
    {
        var response = await sender.Send(query);
        return response.TotalCount == 0 ? TypedResults.NotFound(response) : TypedResults.Ok(response);
    }
    
    public async Task<Ok<TaskResponse[]>> GetTaskWithAllVersions(ISender sender, Guid id)
    {
        var response = await sender.Send(new GetTaskByIdQuery(id));
        return TypedResults.Ok(response);
    }
}
