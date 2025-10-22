using FinalProject.Domain.Events;
using Microsoft.Extensions.Logging;

namespace FinalProject.Application.TodoItems.EventHandlers;

public class TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger)
    : INotificationHandler<TodoItemCreatedEvent>
{
    public Task Handle(TodoItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("FinalProject Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
