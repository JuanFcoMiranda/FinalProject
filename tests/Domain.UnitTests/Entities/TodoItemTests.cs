using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;
using FinalProject.Domain.Events;
using Xunit;

namespace FinalProject.Domain.UnitTests.Entities;

public class TodoItemTests
{
    [Fact]
    public void ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var todoItem = new TodoItem
        {
            Title = "Test Todo",
            Note = "Test Note",
            Priority = PriorityLevel.High,
            Reminder = DateTime.Now.AddDays(1)
        };

        // Assert
        Assert.Equal("Test Todo", todoItem.Title);
        Assert.Equal("Test Note", todoItem.Note);
        Assert.Equal(PriorityLevel.High, todoItem.Priority);
        Assert.NotNull(todoItem.Reminder);
        Assert.False(todoItem.Done);
    }

    [Fact]
    public void ShouldRaiseTodoItemCompletedEventWhenDoneIsSetToTrue()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Title = "Test Todo",
            Done = false
        };

        // Act
        todoItem.Done = true;

        // Assert
        Assert.True(todoItem.Done);
        Assert.Single(todoItem.DomainEvents);
        Assert.IsType<TodoItemCompletedEvent>(todoItem.DomainEvents.First());
    }

    [Fact]
    public void ShouldNotRaiseTodoItemCompletedEventWhenDoneIsAlreadyTrue()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Title = "Test Todo",
            Done = true
        };

        todoItem.ClearDomainEvents();

        // Act
        todoItem.Done = true;

        // Assert
        Assert.True(todoItem.Done);
        Assert.Empty(todoItem.DomainEvents);
    }

    [Fact]
    public void ShouldNotRaiseTodoItemCompletedEventWhenDoneIsSetToFalse()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Title = "Test Todo",
            Done = true
        };

        todoItem.ClearDomainEvents();

        // Act
        todoItem.Done = false;

        // Assert
        Assert.False(todoItem.Done);
        Assert.Empty(todoItem.DomainEvents);
    }
}
