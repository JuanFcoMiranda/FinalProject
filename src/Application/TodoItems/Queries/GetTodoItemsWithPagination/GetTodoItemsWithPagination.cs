using FinalProject.Application.Common.Interfaces;
using FinalProject.Application.Common.Mappings;
using FinalProject.Application.Common.Models;

namespace FinalProject.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public record GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context) : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
    public async Task<PaginatedList<TodoItemBriefDto>> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await context.TodoItems
            .OrderBy(x => x.Title)
  .ProjectToType<TodoItemBriefDto>()
  .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
