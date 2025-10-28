using System.Security.Claims;
using FinalProject.Web.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FinalProject.Web.UnitTests.Services;

public class CurrentUserTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

  public CurrentUserTests()
    {
   _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
    }

    [Fact]
    public void Id_WhenUserIsAuthenticated_ShouldReturnUserId()
    {
        // Arrange
   var userId = "test-user-id";
  var claims = new List<Claim>
        {
        new(ClaimTypes.NameIdentifier, userId)
  };
      var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

   var currentUser = new CurrentUser(_httpContextAccessorMock.Object);

  // Act
        var result = currentUser.Id;

     // Assert
   Assert.Equal(userId, result);
    }

    [Fact]
    public void Id_WhenUserIsNotAuthenticated_ShouldReturnNull()
    {
        // Arrange
   var httpContext = new DefaultHttpContext();
   _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    var currentUser = new CurrentUser(_httpContextAccessorMock.Object);

        // Act
 var result = currentUser.Id;

        // Assert
  Assert.Null(result);
    }

    [Fact]
public void Id_WhenHttpContextIsNull_ShouldReturnNull()
    {
        // Arrange
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

   var currentUser = new CurrentUser(_httpContextAccessorMock.Object);

   // Act
        var result = currentUser.Id;

// Assert
   Assert.Null(result);
    }

    [Fact]
  public void Roles_WhenUserHasRoles_ShouldReturnRoles()
    {
   // Arrange
        var claims = new List<Claim>
        {
   new(ClaimTypes.Role, "Admin"),
            new(ClaimTypes.Role, "User"),
 new(ClaimTypes.Role, "Manager")
        };
      var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
     
var httpContext = new DefaultHttpContext { User = claimsPrincipal };
   _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

     var currentUser = new CurrentUser(_httpContextAccessorMock.Object);

  // Act
 var result = currentUser.Roles;

     // Assert
   Assert.NotNull(result);
   Assert.Equal(3, result.Count);
        Assert.Contains("Admin", result);
        Assert.Contains("User", result);
        Assert.Contains("Manager", result);
    }

    [Fact]
    public void Roles_WhenUserHasNoRoles_ShouldReturnEmptyList()
    {
      // Arrange
        var claims = new List<Claim>
     {
   new(ClaimTypes.NameIdentifier, "test-user-id")
   };
        var identity = new ClaimsIdentity(claims, "TestAuth");
   var claimsPrincipal = new ClaimsPrincipal(identity);
        
   var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

 var currentUser = new CurrentUser(_httpContextAccessorMock.Object);

        // Act
        var result = currentUser.Roles;

     // Assert
   Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Roles_WhenUserIsNotAuthenticated_ShouldReturnEmptyList()
    {
   // Arrange
   var httpContext = new DefaultHttpContext();
  _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

   var currentUser = new CurrentUser(_httpContextAccessorMock.Object);

   // Act
   var result = currentUser.Roles;

      // Assert
    Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Roles_WhenHttpContextIsNull_ShouldReturnNull()
    {
      // Arrange
   _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

    var currentUser = new CurrentUser(_httpContextAccessorMock.Object);

     // Act
        var result = currentUser.Roles;

 // Assert
     Assert.Null(result);
    }
}
