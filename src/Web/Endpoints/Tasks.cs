using Itishnik.Application.Tasks;
using Itishnik.Application.Tasks.Commands.CreateTask;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Itishnik.Web.Endpoints;

public class Tasks : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateTaskRequest);
    }
    
    public async Task<Created<TaskResponse>> CreateTaskRequest(ISender sender, CreateTaskCommand command)
    {
        var response = await sender.Send(command);
        return TypedResults.Created($"/{nameof(Tasks)}/{response.Id}", response);
    }
}
