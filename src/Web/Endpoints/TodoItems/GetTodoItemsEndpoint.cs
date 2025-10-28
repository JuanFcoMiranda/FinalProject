using FastEndpoints;
using FinalProject.Application.Common.Models;
using FinalProject.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace FinalProject.Web.Endpoints.TodoItems;

public class GetTodoItemsEndpoint(ISender sender) : Endpoint<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
    public override void Configure()
    {
        Get("/api/TodoItems");
        AllowAnonymous(); // Change to RequireAuthorization() when auth is enabled
    }

    public override async Task HandleAsync(GetTodoItemsWithPaginationQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        Response = result;
    }
}
