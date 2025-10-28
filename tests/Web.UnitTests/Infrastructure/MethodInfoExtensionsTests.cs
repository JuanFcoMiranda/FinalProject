using System.Reflection;
using Ardalis.GuardClauses;
using FinalProject.Web.Infrastructure;

namespace FinalProject.Web.UnitTests.Infrastructure;

public class MethodInfoExtensionsTests
{
    [Fact]
    public void IsAnonymous_WhenMethodIsAnonymous_ShouldReturnTrue()
    {
        // Arrange
        Func<int, int> anonymousMethod = x => x * 2;
        var methodInfo = anonymousMethod.Method;

        // Act
        var result = methodInfo.IsAnonymous();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAnonymous_WhenMethodIsNamed_ShouldReturnFalse()
    {
        // Arrange
        var methodInfo = typeof(MethodInfoExtensionsTests).GetMethod(nameof(TestMethod))!;

        // Act
        var result = methodInfo.IsAnonymous();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsAnonymous_WhenMethodContainsAngleBrackets_ShouldReturnTrue()
    {
        // Arrange
        Action action = () => { };
        var methodInfo = action.Method;

        // Act
        var result = methodInfo.IsAnonymous();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AnonymousMethod_WhenDelegateIsAnonymous_ShouldThrowArgumentException()
    {
        // Arrange
        var guardClause = Guard.Against;
        Func<int, int> anonymousDelegate = x => x * 2;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
          guardClause.AnonymousMethod(anonymousDelegate));

        Assert.Contains("endpoint name must be specified", exception.Message);
    }

    [Fact]
    public void AnonymousMethod_WhenDelegateIsNamed_ShouldNotThrow()
    {
        // Arrange
        var guardClause = Guard.Against;
        Func<int, int> namedDelegate = NamedMethod;

        // Act & Assert
        var exception = Record.Exception(() =>
    guardClause.AnonymousMethod(namedDelegate));

        Assert.Null(exception);
    }

    // Helper methods
    private void TestMethod()
    {
        // Test method
    }

    private static int NamedMethod(int x)
    {
        return x * 2;
    }
}
