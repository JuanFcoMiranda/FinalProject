using FinalProject.Domain.Entities;
using FinalProject.Domain.Events;
using Xunit;

namespace FinalProject.Domain.UnitTests.Events;

public class TodoItemEventsTests
{
    [Fact]
    public void TodoItemCreatedEventShouldHoldItemReference()
    {
        // Arrange
        var todoItem = new TodoItem { Title = "Test Todo" };

        // Act
        var domainEvent = new TodoItemCreatedEvent(todoItem);

        // Assert
        Assert.NotNull(domainEvent.Item);
        Assert.Equal(todoItem, domainEvent.Item);
        Assert.Equal("Test Todo", domainEvent.Item.Title);
    }

    [Fact]
    public void TodoItemCompletedEventShouldHoldItemReference()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Title = "Test Todo",
            Done = true
        };

        // Act
        var domainEvent = new TodoItemCompletedEvent(todoItem);

        // Assert
        Assert.NotNull(domainEvent.Item);
        Assert.Equal(todoItem, domainEvent.Item);
        Assert.True(domainEvent.Item.Done);
    }

    [Fact]
    public void TodoItemDeletedEventShouldHoldItemReference()
    {
        // Arrange
        var todoItem = new TodoItem { Title = "Test Todo" };

        // Act
        var domainEvent = new TodoItemDeletedEvent(todoItem);

        // Assert
        Assert.NotNull(domainEvent.Item);
        Assert.Equal(todoItem, domainEvent.Item);
    }
}
