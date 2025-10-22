using FinalProject.Application.Common.Interfaces;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Events;

namespace FinalProject.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : IRequest<int>
{
    public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateTodoItemCommand, int>
{
    public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            Title = request.Title,
            Done = false
        };

        entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

        context.TodoItems.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
