using FinalProject.Application.TodoItems.Commands.UpdateTodoItem;
using FastEndpoints;

namespace FinalProject.Web.Endpoints.TodoItems;

public class UpdateTodoItemRequest
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool Done { get; set; }
}

public class UpdateTodoItemEndpoint(ISender sender) : Endpoint<UpdateTodoItemRequest>
{
    public override void Configure()
    {
        Put("/api/TodoItems/{id}");
        AllowAnonymous(); // Change to RequireAuthorization() when auth is enabled
    }

    public override async Task HandleAsync(UpdateTodoItemRequest req, CancellationToken ct)
    {
        var command = new UpdateTodoItemCommand
        {
            Id = req.Id,
            Title = req.Title,
            Done = req.Done
        };

        await sender.Send(command, ct);
        HttpContext.Response.StatusCode = 204;
    }
}
