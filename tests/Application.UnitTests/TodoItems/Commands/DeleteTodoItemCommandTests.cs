using FinalProject.Application.TodoItems.Commands.DeleteTodoItem;
using Shouldly;

namespace FinalProject.Application.UnitTests.TodoItems.Commands;

public class DeleteTodoItemCommandTests
{
    [Fact]
    public void DeleteTodoItemCommand_ShouldHaveIdProperty()
    {
        // Arrange & Act
        var command = new DeleteTodoItemCommand(42);

        // Assert
        command.Id.ShouldBe(42);
    }

    [Fact]
    public void DeleteTodoItemCommand_ShouldAcceptZeroId()
    {
        // Arrange & Act
        var command = new DeleteTodoItemCommand(0);

        // Assert
        command.Id.ShouldBe(0);
    }

    [Fact]
    public void DeleteTodoItemCommand_ShouldBeRecord()
    {
        // Arrange & Act
        var command1 = new DeleteTodoItemCommand(10);
        var command2 = new DeleteTodoItemCommand(10);

        // Assert
        command1.ShouldBe(command2); // Records have value equality
    }

    [Fact]
    public void DeleteTodoItemCommand_DifferentIds_ShouldNotBeEqual()
    {
        // Arrange & Act
        var command1 = new DeleteTodoItemCommand(1);
        var command2 = new DeleteTodoItemCommand(2);

        // Assert
        command1.ShouldNotBe(command2);
    }
}
