using FinalProject.Application.TodoItems.Commands.CreateTodoItem;
using Shouldly;

namespace FinalProject.Application.UnitTests.TodoItems.Commands;

public class CreateTodoItemCommandTests
{
    [Fact]
    public void CreateTodoItemCommand_ShouldHaveTitleProperty()
    {
        // Arrange & Act
        var command = new CreateTodoItemCommand { Title = "Test Todo" };

        // Assert
        command.Title.ShouldBe("Test Todo");
    }

    [Fact]
    public void CreateTodoItemCommand_ShouldAllowNullTitle()
    {
        // Arrange & Act
        var command = new CreateTodoItemCommand { Title = null };

        // Assert
        command.Title.ShouldBeNull();
    }

    [Fact]
    public void CreateTodoItemCommand_ShouldAllowEmptyTitle()
    {
        // Arrange & Act
        var command = new CreateTodoItemCommand { Title = "" };

        // Assert
        command.Title.ShouldBe("");
    }

    [Fact]
    public void CreateTodoItemCommand_ShouldBeRecord()
    {
        // Arrange & Act
        var command1 = new CreateTodoItemCommand { Title = "Test" };
        var command2 = new CreateTodoItemCommand { Title = "Test" };

        // Assert
        command1.ShouldBe(command2); // Records have value equality
    }
}
