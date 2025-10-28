using FinalProject.Application.Common.Models;
using Xunit;

namespace FinalProject.Application.UnitTests.Common.Models;

public class PaginatedListTests
{
    [Fact]
public void ShouldCreatePaginatedListWithCorrectProperties()
    {
    // Arrange
  var items = new List<string> { "Item1", "Item2", "Item3" };
  const int totalCount = 10;
   const int pageNumber = 1;
 const int pageSize = 3;

        // Act
  var paginatedList = new PaginatedList<string>(items, totalCount, pageNumber, pageSize);

      // Assert
     Assert.Equal(3, paginatedList.Items.Count);
        Assert.Equal(1, paginatedList.PageNumber);
   Assert.Equal(4, paginatedList.TotalPages); // Ceiling(10/3) = 4
        Assert.Equal(10, paginatedList.TotalCount);
    }

    [Fact]
    public void ShouldCalculateHasPreviousPageCorrectly()
    {
// Arrange
var items = new List<string> { "Item1", "Item2" };

        // Act
        var firstPage = new PaginatedList<string>(items, 10, 1, 2);
    var secondPage = new PaginatedList<string>(items, 10, 2, 2);

        // Assert
 Assert.False(firstPage.HasPreviousPage);
        Assert.True(secondPage.HasPreviousPage);
    }

    [Fact]
    public void ShouldCalculateHasNextPageCorrectly()
    {
   // Arrange
  var items = new List<string> { "Item1", "Item2" };

     // Act
  var firstPage = new PaginatedList<string>(items, 10, 1, 2);
        var lastPage = new PaginatedList<string>(items, 10, 5, 2);

        // Assert
        Assert.True(firstPage.HasNextPage);
   Assert.False(lastPage.HasNextPage);
}

  [Fact]
    public void ShouldCalculateTotalPagesCorrectly()
    {
        // Arrange & Act
   var result1 = new PaginatedList<string>([], 10, 1, 3); // 10/3 = 3.33 -> 4 pages
        var result2 = new PaginatedList<string>([], 9, 1, 3);  // 9/3 = 3 pages
  var result3 = new PaginatedList<string>([], 0, 1, 3);  // 0/3 = 0 pages

        // Assert
   Assert.Equal(4, result1.TotalPages);
        Assert.Equal(3, result2.TotalPages);
        Assert.Equal(0, result3.TotalPages);
    }

    [Fact]
    public void ShouldHandleFirstPageScenario()
    {
      // Arrange
 var items = Enumerable.Range(1, 10).ToList();
        const int totalCount = 50;
      const int pageNumber = 1;
        const int pageSize = 10;

        // Act
     var result = new PaginatedList<int>(items, totalCount, pageNumber, pageSize);

        // Assert
  Assert.False(result.HasPreviousPage);
   Assert.True(result.HasNextPage);
        Assert.Equal(1, result.Items.First());
    }

    [Fact]
    public void ShouldHandleLastPageScenario()
    {
     // Arrange
        var items = Enumerable.Range(21, 5).ToList(); // Items 21-25
     const int totalCount = 25;
        const int pageNumber = 3;
        const int pageSize = 10;

        // Act
        var result = new PaginatedList<int>(items, totalCount, pageNumber, pageSize);

    // Assert
      Assert.True(result.HasPreviousPage);
 Assert.False(result.HasNextPage);
     Assert.Equal(5, result.Items.Count); // Last page has only 5 items
    }

    [Fact]
    public void ShouldHandleEmptyList()
  {
        // Arrange
   var items = new List<int>();
     const int totalCount = 0;
        const int pageNumber = 1;
    const int pageSize = 10;

        // Act
        var result = new PaginatedList<int>(items, totalCount, pageNumber, pageSize);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasPreviousPage);
   Assert.False(result.HasNextPage);
    }

    [Fact]
    public void ShouldHandleSinglePageScenario()
    {
// Arrange
        var items = Enumerable.Range(1, 5).ToList();
        const int totalCount = 5;
        const int pageNumber = 1;
     const int pageSize = 10;

        // Act
        var result = new PaginatedList<int>(items, totalCount, pageNumber, pageSize);

        // Assert
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(5, result.Items.Count);
    }

    [Fact]
    public void ShouldHandleMiddlePageScenario()
    {
        // Arrange
        var items = Enumerable.Range(11, 10).ToList(); // Items 11-20
        const int totalCount = 50;
        const int pageNumber = 2;
        const int pageSize = 10;

 // Act
        var result = new PaginatedList<int>(items, totalCount, pageNumber, pageSize);

      // Assert
        Assert.True(result.HasPreviousPage);
 Assert.True(result.HasNextPage);
        Assert.Equal(5, result.TotalPages);
        Assert.Equal(10, result.Items.Count);
     Assert.Equal(11, result.Items.First());
    }
}
