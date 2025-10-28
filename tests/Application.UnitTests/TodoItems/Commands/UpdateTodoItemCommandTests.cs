using FinalProject.Application.TodoItems.Commands.UpdateTodoItem;
using Shouldly;

namespace FinalProject.Application.UnitTests.TodoItems.Commands;

public class UpdateTodoItemCommandTests
{
    [Fact]
    public void UpdateTodoItemCommand_ShouldHaveAllProperties()
    {
// Arrange & Act
     var command = new UpdateTodoItemCommand
        {
  Id = 1,
            Title = "Updated Title",
Done = true
        };

        // Assert
        command.Id.ShouldBe(1);
        command.Title.ShouldBe("Updated Title");
   command.Done.ShouldBeTrue();
    }

    [Fact]
    public void UpdateTodoItemCommand_ShouldAllowNullTitle()
    {
   // Arrange & Act
   var command = new UpdateTodoItemCommand
        {
   Id = 1,
    Title = null,
Done = false
        };

        // Assert
      command.Title.ShouldBeNull();
    }

    [Fact]
    public void UpdateTodoItemCommand_ShouldHandleDoneFalse()
    {
        // Arrange & Act
        var command = new UpdateTodoItemCommand
        {
  Id = 5,
        Title = "Test",
   Done = false
        };

        // Assert
      command.Done.ShouldBeFalse();
    }

  [Fact]
    public void UpdateTodoItemCommand_ShouldBeRecord()
    {
        // Arrange & Act
        var command1 = new UpdateTodoItemCommand { Id = 1, Title = "Test", Done = true };
  var command2 = new UpdateTodoItemCommand { Id = 1, Title = "Test", Done = true };

// Assert
  command1.ShouldBe(command2); // Records have value equality
    }
}
