using FinalProject.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Shouldly;

namespace FinalProject.Infrastructure.UnitTests.Identity;

public class IdentityResultExtensionsTests
{
    [Fact]
    public void ToApplicationResult_WhenSucceeded_ShouldReturnSuccessResult()
    {
        // Arrange
        var identityResult = IdentityResult.Success;

        // Act
      var result = identityResult.ToApplicationResult();

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void ToApplicationResult_WhenFailed_ShouldReturnFailureResult()
    {
        // Arrange
        var errors = new[]
        {
    new IdentityError { Code = "Error1", Description = "First error" },
  new IdentityError { Code = "Error2", Description = "Second error" }
  };
        var identityResult = IdentityResult.Failed(errors);

      // Act
        var result = identityResult.ToApplicationResult();

      // Assert
        result.Succeeded.ShouldBeFalse();
        result.Errors.Length.ShouldBe(2);
        result.Errors.ShouldContain("First error");
   result.Errors.ShouldContain("Second error");
    }

    [Fact]
    public void ToApplicationResult_WhenFailedWithSingleError_ShouldReturnFailureResult()
    {
        // Arrange
     var error = new IdentityError { Code = "TestError", Description = "Test error description" };
        var identityResult = IdentityResult.Failed(error);

        // Act
        var result = identityResult.ToApplicationResult();

     // Assert
   result.Succeeded.ShouldBeFalse();
    result.Errors.Length.ShouldBe(1);
        result.Errors[0].ShouldBe("Test error description");
    }

    [Fact]
public void ToApplicationResult_WhenFailedWithNoErrors_ShouldReturnFailureResultWithEmptyErrors()
    {
        // Arrange
        var identityResult = IdentityResult.Failed();

        // Act
      var result = identityResult.ToApplicationResult();

        // Assert
 result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldBeEmpty();
    }
}
