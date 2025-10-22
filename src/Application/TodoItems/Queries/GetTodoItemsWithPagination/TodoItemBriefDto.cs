namespace FinalProject.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class TodoItemBriefDto
{
    public int Id { get; init; }

    public string? Title { get; init; }

    public bool Done { get; init; }

    // Mapster will automatically map properties by convention
    // No need for manual configuration for simple property mappings
}
