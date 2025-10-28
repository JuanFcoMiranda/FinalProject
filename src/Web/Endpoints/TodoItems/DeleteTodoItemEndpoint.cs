using FastEndpoints;
using FinalProject.Application.TodoItems.Commands.DeleteTodoItem;

namespace FinalProject.Web.Endpoints.TodoItems;

public class DeleteTodoItemRequest
{
    public int Id { get; set; }
}

public class DeleteTodoItemEndpoint(ISender sender) : Endpoint<DeleteTodoItemRequest>
{
    public override void Configure()
    {
        Delete("/api/TodoItems/{id}");
        AllowAnonymous(); // Change to RequireAuthorization() when auth is enabled
    }

    public override async Task HandleAsync(DeleteTodoItemRequest req, CancellationToken ct)
    {
        await sender.Send(new DeleteTodoItemCommand(req.Id), ct);
        HttpContext.Response.StatusCode = 204;
    }
}
