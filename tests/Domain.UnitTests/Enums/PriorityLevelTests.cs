using FinalProject.Domain.Enums;
using Xunit;

namespace FinalProject.Domain.UnitTests.Enums;

public class PriorityLevelTests
{
    [Fact]
public void ShouldHaveCorrectValues()
    {
  // Assert
   Assert.Equal(0, (int)PriorityLevel.None);
        Assert.Equal(1, (int)PriorityLevel.Low);
        Assert.Equal(2, (int)PriorityLevel.Medium);
        Assert.Equal(3, (int)PriorityLevel.High);
    }

    [Theory]
    [InlineData(PriorityLevel.None)]
 [InlineData(PriorityLevel.Low)]
    [InlineData(PriorityLevel.Medium)]
 [InlineData(PriorityLevel.High)]
    public void ShouldBeDefinedEnum(PriorityLevel priority)
    {
   // Assert
        Assert.True(Enum.IsDefined(typeof(PriorityLevel), priority));
    }
}
