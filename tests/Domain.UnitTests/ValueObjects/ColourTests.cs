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
}
