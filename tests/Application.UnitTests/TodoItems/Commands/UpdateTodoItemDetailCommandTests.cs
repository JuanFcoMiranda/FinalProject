using FinalProject.Application.TodoItems.Commands.UpdateTodoItemDetail;
using FinalProject.Domain.Enums;
using Shouldly;

namespace FinalProject.Application.UnitTests.TodoItems.Commands;

public class UpdateTodoItemDetailCommandTests
{
    [Fact]
    public void UpdateTodoItemDetailCommand_ShouldHaveAllProperties()
    {
   // Arrange & Act
   var command = new UpdateTodoItemDetailCommand
  {
   Id = 1,
     Priority = PriorityLevel.High,
Note = "Important task"
  };

        // Assert
    command.Id.ShouldBe(1);
  command.Priority.ShouldBe(PriorityLevel.High);
        command.Note.ShouldBe("Important task");
    }

    [Fact]
    public void UpdateTodoItemDetailCommand_ShouldAllowNullNote()
    {
        // Arrange & Act
      var command = new UpdateTodoItemDetailCommand
   {
   Id = 5,
        Priority = PriorityLevel.None,
   Note = null
};

     // Assert
      command.Note.ShouldBeNull();
}

    [Fact]
    public void UpdateTodoItemDetailCommand_ShouldHandleAllPriorityLevels()
    {
        // Arrange & Act
     var commandNone = new UpdateTodoItemDetailCommand { Priority = PriorityLevel.None };
      var commandLow = new UpdateTodoItemDetailCommand { Priority = PriorityLevel.Low };
   var commandMedium = new UpdateTodoItemDetailCommand { Priority = PriorityLevel.Medium };
     var commandHigh = new UpdateTodoItemDetailCommand { Priority = PriorityLevel.High };

        // Assert
   commandNone.Priority.ShouldBe(PriorityLevel.None);
   commandLow.Priority.ShouldBe(PriorityLevel.Low);
   commandMedium.Priority.ShouldBe(PriorityLevel.Medium);
  commandHigh.Priority.ShouldBe(PriorityLevel.High);
    }

    [Fact]
    public void UpdateTodoItemDetailCommand_ShouldBeRecord()
    {
        // Arrange & Act
      var command1 = new UpdateTodoItemDetailCommand
        {
  Id = 1,
    Priority = PriorityLevel.Medium,
Note = "Test"
 };
    var command2 = new UpdateTodoItemDetailCommand
        {
   Id = 1,
   Priority = PriorityLevel.Medium,
  Note = "Test"
 };

     // Assert
        command1.ShouldBe(command2); // Records have value equality
    }
}
