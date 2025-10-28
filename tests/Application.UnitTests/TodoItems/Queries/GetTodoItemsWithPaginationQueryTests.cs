using FinalProject.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Shouldly;

namespace FinalProject.Application.UnitTests.TodoItems.Queries;

public class GetTodoItemsWithPaginationQueryTests
{
    [Fact]
    public void GetTodoItemsWithPaginationQuery_ShouldHaveDefaultValues()
 {
     // Arrange & Act
     var query = new GetTodoItemsWithPaginationQuery();

        // Assert
   query.PageNumber.ShouldBe(1);
        query.PageSize.ShouldBe(10);
    }

    [Fact]
    public void GetTodoItemsWithPaginationQuery_ShouldAllowCustomPageNumber()
    {
        // Arrange & Act
        var query = new GetTodoItemsWithPaginationQuery { PageNumber = 5 };

   // Assert
 query.PageNumber.ShouldBe(5);
    }

    [Fact]
    public void GetTodoItemsWithPaginationQuery_ShouldAllowCustomPageSize()
    {
   // Arrange & Act
        var query = new GetTodoItemsWithPaginationQuery { PageSize = 25 };

        // Assert
        query.PageSize.ShouldBe(25);
    }

    [Fact]
    public void GetTodoItemsWithPaginationQuery_ShouldAllowBothCustomValues()
    {
  // Arrange & Act
     var query = new GetTodoItemsWithPaginationQuery
        {
     PageNumber = 3,
  PageSize = 50
        };

        // Assert
query.PageNumber.ShouldBe(3);
  query.PageSize.ShouldBe(50);
    }

    [Fact]
    public void GetTodoItemsWithPaginationQuery_ShouldBeRecord()
    {
   // Arrange & Act
        var query1 = new GetTodoItemsWithPaginationQuery { PageNumber = 2, PageSize = 20 };
  var query2 = new GetTodoItemsWithPaginationQuery { PageNumber = 2, PageSize = 20 };

        // Assert
 query1.ShouldBe(query2); // Records have value equality
    }
}
