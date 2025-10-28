using FinalProject.Application.Common.Models;
using Xunit;

namespace FinalProject.Application.UnitTests.Common.Models;

public class ResultTests
{
    [Fact]
    public void SuccessShouldCreateSuccessfulResult()
    {
        // Act
     var result = Result.Success();

    // Assert
   Assert.True(result.Succeeded);
 Assert.Empty(result.Errors);
    }

    [Fact]
    public void FailureShouldCreateFailedResultWithErrors()
    {
        // Arrange
        var errors = new[] { "Error 1", "Error 2" };

        // Act
        var result = Result.Failure(errors);

        // Assert
      Assert.False(result.Succeeded);
   Assert.Equal(2, result.Errors.Length);
        Assert.Contains("Error 1", result.Errors);
     Assert.Contains("Error 2", result.Errors);
    }

    [Fact]
    public void FailureShouldCreateFailedResultWithSingleError()
    {
      // Arrange
        var errors = new[] { "Single Error" };

        // Act
        var result = Result.Failure(errors);

 // Assert
        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);
    Assert.Equal("Single Error", result.Errors[0]);
    }

    [Fact]
    public void FailureShouldCreateFailedResultWithNoErrors()
    {
        // Arrange
      var errors = Array.Empty<string>();

// Act
        var result = Result.Failure(errors);

        // Assert
        Assert.False(result.Succeeded);
  Assert.Empty(result.Errors);
    }
}
