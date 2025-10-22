using FinalProject.Domain.Events;
using Microsoft.Extensions.Logging;

namespace FinalProject.Application.TodoItems.EventHandlers;

public class TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger)
    : INotificationHandler<TodoItemCompletedEvent>
{
    public Task Handle(TodoItemCompletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("FinalProject Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
