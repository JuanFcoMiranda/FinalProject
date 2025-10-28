using FinalProject.Application.Common.Models;
using Xunit;

namespace FinalProject.Application.UnitTests.Common.Models;

public class LookupDtoTests
{
[Fact]
    public void ShouldCreateLookupDtoWithProperties()
    {
        // Act
        var dto = new LookupDto
        {
            Id = 1,
       Title = "Test Title"
        };

        // Assert
   Assert.Equal(1, dto.Id);
        Assert.Equal("Test Title", dto.Title);
    }

    [Fact]
    public void ShouldAllowNullTitle()
    {
        // Act
        var dto = new LookupDto
        {
            Id = 1,
   Title = null
        };

        // Assert
      Assert.Equal(1, dto.Id);
     Assert.Null(dto.Title);
    }
}
