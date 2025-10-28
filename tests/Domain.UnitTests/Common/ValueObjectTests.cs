using FinalProject.Domain.ValueObjects;
using Xunit;

namespace FinalProject.Domain.UnitTests.Common;

public class ValueObjectTests
{
    [Fact]
    public void EqualValueObjectsShouldBeEqual()
    {
        // Arrange
        var colour1 = Colour.White;
        var colour2 = Colour.From("#FFFFFF");

        // Act & Assert
        Assert.Equal(colour1, colour2);
        Assert.True(colour1.Equals(colour2));
    }

    [Fact]
    public void DifferentValueObjectsShouldNotBeEqual()
    {
        // Arrange
        var colour1 = Colour.White;
        var colour2 = Colour.Red;

        // Act & Assert
        Assert.NotEqual(colour1, colour2);
        Assert.False(colour1.Equals(colour2));
    }

    [Fact]
    public void ValueObjectShouldNotEqualNull()
    {
        // Arrange
        var colour = Colour.White;

        // Act & Assert
        Assert.False(colour.Equals(null));
    }

    [Fact]
    public void ValueObjectShouldNotEqualDifferentType()
    {
        // Arrange
        var colour = Colour.White;
        var someString = "test";

        // Act & Assert
        Assert.False(colour.Equals(someString));
    }

    [Fact]
    public void EqualValueObjectsShouldHaveSameHashCode()
    {
        // Arrange
        var colour1 = Colour.White;
        var colour2 = Colour.From("#FFFFFF");

        // Act & Assert
        Assert.Equal(colour1.GetHashCode(), colour2.GetHashCode());
    }

    [Fact]
    public void DifferentValueObjectsShouldHaveDifferentHashCode()
    {
        // Arrange
        var colour1 = Colour.White;
        var colour2 = Colour.Red;

        // Act & Assert
        Assert.NotEqual(colour1.GetHashCode(), colour2.GetHashCode());
    }
}
