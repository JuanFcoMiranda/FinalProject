using FinalProject.Domain.Exceptions;
using FinalProject.Domain.ValueObjects;
using Xunit;

namespace FinalProject.Domain.UnitTests.ValueObjects;

public class ColourTests
{
    [Fact]
    public void ShouldReturnCorrectColourCode()
    {
        const string code = "#FFFFFF";

        var colour = Colour.From(code);

        Assert.Equal(code, colour.Code);
    }

    [Fact]
    public void ToStringReturnsCode()
    {
        var colour = Colour.White;

        Assert.Equal(colour.Code, colour.ToString());
    }

    [Fact]
    public void ShouldPerformImplicitConversionToColourCodeString()
    {
        string code = Colour.White;

        Assert.Equal("#FFFFFF", code);
    }

    [Fact]
    public void ShouldPerformExplicitConversionGivenSupportedColourCode()
    {
        var colour = (Colour)"#FFFFFF";

        Assert.Equal(Colour.White, colour);
    }

    [Fact]
    public void ShouldThrowUnsupportedColourExceptionGivenNotSupportedColourCode()
    {
        Assert.Throws<UnsupportedColourException>(() => Colour.From("##FF33CC"));
    }

    [Fact]
    public void ShouldReturnCorrectPredefinedColours()
    {
        Assert.Equal("#FFFFFF", Colour.White.Code);
        Assert.Equal("#FF5733", Colour.Red.Code);
        Assert.Equal("#FFC300", Colour.Orange.Code);
        Assert.Equal("#FFFF66", Colour.Yellow.Code);
        Assert.Equal("#CCFF99", Colour.Green.Code);
        Assert.Equal("#6666FF", Colour.Blue.Code);
        Assert.Equal("#9966CC", Colour.Purple.Code);
        Assert.Equal("#999999", Colour.Grey.Code);
    }

    [Fact]
    public void ShouldHandleNullOrWhiteSpaceCodeByDefaultingToBlack()
    {
        var colourWithNull = new Colour(null!);
        var colourWithEmpty = new Colour("");
        var colourWithWhiteSpace = new Colour("   ");

        Assert.Equal("#000000", colourWithNull.Code);
        Assert.Equal("#000000", colourWithEmpty.Code);
        Assert.Equal("#000000", colourWithWhiteSpace.Code);
    }

    [Theory]
    [InlineData("#FF5733")]
    [InlineData("#FFC300")]
    [InlineData("#FFFF66")]
    [InlineData("#CCFF99")]
    [InlineData("#6666FF")]
    [InlineData("#9966CC")]
    [InlineData("#999999")]
    public void ShouldCreateColourFromAllSupportedCodes(string code)
    {
        var colour = Colour.From(code);

        Assert.Equal(code, colour.Code);
    }

    [Fact]
    public void TwoColoursWithSameCodeShouldBeEqual()
    {
        var colour1 = Colour.From("#FFFFFF");
        var colour2 = Colour.From("#FFFFFF");

        Assert.Equal(colour1, colour2);
    }
}
