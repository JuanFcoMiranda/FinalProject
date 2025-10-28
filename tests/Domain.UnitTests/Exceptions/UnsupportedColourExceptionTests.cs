using FinalProject.Domain.Exceptions;
using Xunit;

namespace FinalProject.Domain.UnitTests.Exceptions;

public class UnsupportedColourExceptionTests
{
    [Fact]
    public void ShouldCreateExceptionWithCorrectMessage()
    {
        // Arrange
 const string code = "#INVALID";

        // Act
        var exception = new UnsupportedColourException(code);

   // Assert
   Assert.Equal("Colour \"#INVALID\" is unsupported.", exception.Message);
    }

    [Fact]
    public void ShouldBeExceptionType()
    {
        // Act
        var exception = new UnsupportedColourException("#TEST");

 // Assert
  Assert.IsAssignableFrom<Exception>(exception);
    }
}
