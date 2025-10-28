using FastEndpoints;
using FinalProject.Application.TodoItems.Commands.CreateTodoItem;

namespace FinalProject.Web.Endpoints.TodoItems;

public class CreateTodoItemEndpoint(ISender sender) : Endpoint<CreateTodoItemCommand, int>
{
    public override void Configure()
    {
        Post("/api/TodoItems");
        AllowAnonymous(); // Change to RequireAuthorization() when auth is enabled
    }

    public override async Task HandleAsync(CreateTodoItemCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        Response = id;
        HttpContext.Response.StatusCode = 201;
    }
}
