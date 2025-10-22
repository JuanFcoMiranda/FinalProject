using FinalProject.Application.TodoItems.Commands.UpdateTodoItemDetail;
using FastEndpoints;

namespace FinalProject.Web.Endpoints.TodoItems;

public class UpdateTodoItemDetailRequest
{
    public int Id { get; set; }
    public UpdateTodoItemDetailCommand Command { get; set; } = default!;
}

public class UpdateTodoItemDetailEndpoint(ISender sender) : Endpoint<UpdateTodoItemDetailRequest>
{
    public override void Configure()
    {
        Put("/api/TodoItems/UpdateDetail/{id}");
        AllowAnonymous(); // Change to RequireAuthorization() when auth is enabled
    }

    public override async Task HandleAsync(UpdateTodoItemDetailRequest req, CancellationToken ct)
    {
        if (req.Id != req.Command.Id)
        {
            AddError("Id mismatch");
            ThrowIfAnyErrors();
        }

        await sender.Send(req.Command, ct);
        HttpContext.Response.StatusCode = 204;
    }
}
