using FinalProject.Application.Common.Exceptions;
using Xunit;

namespace FinalProject.Application.UnitTests.Common.Exceptions;

public class ForbiddenAccessExceptionTests
{
    [Fact]
    public void ShouldCreateForbiddenAccessException()
    {
        // Act
        var exception = new ForbiddenAccessException();

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ForbiddenAccessException>(exception);
    }

    [Fact]
    public void ShouldBeExceptionType()
    {
        // Act
        var exception = new ForbiddenAccessException();

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }
}
