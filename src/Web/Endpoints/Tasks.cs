using Itishnik.Application.Common.Models;
using Itishnik.Application.Tasks;
using Itishnik.Application.Tasks.Commands.CreateTask;
using Itishnik.Application.Tasks.Queries.GetTaskList;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Itishnik.Web.Endpoints;

public class Tasks : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateTaskRequest)
            .MapGet(GetTaskList);
    }
    
    public async Task<Created<TaskResponse>> CreateTaskRequest(ISender sender, CreateTaskCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Tasks)}/{response.Id}", response);
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
}
