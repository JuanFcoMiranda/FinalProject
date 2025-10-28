using FinalProject.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Xunit;

namespace FinalProject.Application.UnitTests.TodoItems.Queries;

public class TodoItemBriefDtoTests
{
 [Fact]
    public void ShouldCreateTodoItemBriefDtoWithProperties()
    {
        // Act
        var dto = new TodoItemBriefDto
        {
Id = 1,
 Title = "Test Todo",
            Done = true
 };

     // Assert
   Assert.Equal(1, dto.Id);
  Assert.Equal("Test Todo", dto.Title);
        Assert.True(dto.Done);
    }

    [Fact]
    public void ShouldAllowNullTitle()
    {
    // Act
        var dto = new TodoItemBriefDto
        {
   Id = 1,
         Title = null,
Done = false
        };

        // Assert
        Assert.Equal(1, dto.Id);
   Assert.Null(dto.Title);
        Assert.False(dto.Done);
    }
}
