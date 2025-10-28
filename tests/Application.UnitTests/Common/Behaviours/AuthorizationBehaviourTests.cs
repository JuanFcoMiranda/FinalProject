using FinalProject.Application.Common.Behaviours;
using FinalProject.Application.Common.Exceptions;
using FinalProject.Application.Common.Interfaces;
using FinalProject.Application.Common.Security;
using MediatR;
using Moq;

namespace FinalProject.Application.UnitTests.Common.Behaviours;

public class AuthorizationBehaviourTests
{
    private readonly Mock<IUser> _userMock;
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly Mock<RequestHandlerDelegate<Unit>> _nextMock;
    private readonly AuthorizationBehaviour<TestRequest, Unit> _behaviour;

    public AuthorizationBehaviourTests()
    {
        _userMock = new Mock<IUser>();
        _identityServiceMock = new Mock<IIdentityService>();
        _nextMock = new Mock<RequestHandlerDelegate<Unit>>();
        _nextMock.Setup(x => x()).ReturnsAsync(Unit.Value);

        _behaviour = new AuthorizationBehaviour<TestRequest, Unit>(
   _userMock.Object,
            _identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenNoAuthorizeAttribute_ShouldCallNext()
    {
 // Arrange
        var request = new TestRequestWithoutAuth();
     var behaviour = new AuthorizationBehaviour<TestRequestWithoutAuth, Unit>(
    _userMock.Object,
          _identityServiceMock.Object);

     // Act
        await behaviour.Handle(request, _nextMock.Object, CancellationToken.None);

// Assert
 _nextMock.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
   var request = new TestRequestWithRole(); // Usar una clase con [Authorize]
        var behaviour = new AuthorizationBehaviour<TestRequestWithRole, Unit>(
    _userMock.Object,
     _identityServiceMock.Object);
        
        _userMock.Setup(x => x.Id).Returns((string?)null);

    // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
     behaviour.Handle(request, _nextMock.Object, CancellationToken.None));

        _nextMock.Verify(x => x(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsAuthenticated_AndNoRolesOrPolicies_ShouldCallNext()
    {
     // Arrange
     var request = new TestRequestWithEmptyAuthorize();
        var behaviour = new AuthorizationBehaviour<TestRequestWithEmptyAuthorize, Unit>(
            _userMock.Object,
     _identityServiceMock.Object);

        _userMock.Setup(x => x.Id).Returns("test-user-id");

        // Act
        await behaviour.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(x => x(), Times.Once);
    }

    [Fact]
public async Task Handle_WhenUserHasRequiredRole_ShouldCallNext()
    {
     // Arrange
        var request = new TestRequestWithRole();
     var behaviour = new AuthorizationBehaviour<TestRequestWithRole, Unit>(
    _userMock.Object,
      _identityServiceMock.Object);

    _userMock.Setup(x => x.Id).Returns("test-user-id");
 _userMock.Setup(x => x.Roles).Returns(new List<string> { "Admin" });

      // Act
        await behaviour.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
   _nextMock.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotHaveRequiredRole_ShouldThrowForbiddenAccessException()
    {
        // Arrange
        var request = new TestRequestWithRole();
        var behaviour = new AuthorizationBehaviour<TestRequestWithRole, Unit>(
            _userMock.Object,
     _identityServiceMock.Object);

        _userMock.Setup(x => x.Id).Returns("test-user-id");
 _userMock.Setup(x => x.Roles).Returns(new List<string> { "User" });

        // Act & Assert
      await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
 behaviour.Handle(request, _nextMock.Object, CancellationToken.None));

        _nextMock.Verify(x => x(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserHasOneOfMultipleRoles_ShouldCallNext()
    {
        // Arrange
  var request = new TestRequestWithMultipleRoles();
        var behaviour = new AuthorizationBehaviour<TestRequestWithMultipleRoles, Unit>(
  _userMock.Object,
            _identityServiceMock.Object);

        _userMock.Setup(x => x.Id).Returns("test-user-id");
        _userMock.Setup(x => x.Roles).Returns(new List<string> { "Manager" });

        // Act
      await behaviour.Handle(request, _nextMock.Object, CancellationToken.None);

    // Assert
        _nextMock.Verify(x => x(), Times.Once);
 }

    [Fact]
    public async Task Handle_WhenUserHasNoRoles_ShouldThrowForbiddenAccessException()
    {
        // Arrange
        var request = new TestRequestWithRole();
        var behaviour = new AuthorizationBehaviour<TestRequestWithRole, Unit>(
    _userMock.Object,
    _identityServiceMock.Object);

        _userMock.Setup(x => x.Id).Returns("test-user-id");
        _userMock.Setup(x => x.Roles).Returns((List<string>?)null);

        // Act & Assert
    await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
    behaviour.Handle(request, _nextMock.Object, CancellationToken.None));

    _nextMock.Verify(x => x(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPolicyIsAuthorized_ShouldCallNext()
    {
 // Arrange
        var request = new TestRequestWithPolicy();
        var behaviour = new AuthorizationBehaviour<TestRequestWithPolicy, Unit>(
       _userMock.Object,
 _identityServiceMock.Object);

  _userMock.Setup(x => x.Id).Returns("test-user-id");
        _identityServiceMock.Setup(x => x.AuthorizeAsync("test-user-id", "CanEdit"))
            .ReturnsAsync(true);

        // Act
        await behaviour.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(x => x(), Times.Once);
        _identityServiceMock.Verify(x => x.AuthorizeAsync("test-user-id", "CanEdit"), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPolicyIsNotAuthorized_ShouldThrowForbiddenAccessException()
    {
        // Arrange
   var request = new TestRequestWithPolicy();
        var behaviour = new AuthorizationBehaviour<TestRequestWithPolicy, Unit>(
            _userMock.Object,
            _identityServiceMock.Object);

        _userMock.Setup(x => x.Id).Returns("test-user-id");
        _identityServiceMock.Setup(x => x.AuthorizeAsync("test-user-id", "CanEdit"))
       .ReturnsAsync(false);

        // Act & Assert
     await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            behaviour.Handle(request, _nextMock.Object, CancellationToken.None));

        _nextMock.Verify(x => x(), Times.Never);
        _identityServiceMock.Verify(x => x.AuthorizeAsync("test-user-id", "CanEdit"), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenMultiplePoliciesAreAuthorized_ShouldCallNext()
    {
        // Arrange
        var request = new TestRequestWithMultiplePolicies();
        var behaviour = new AuthorizationBehaviour<TestRequestWithMultiplePolicies, Unit>(
            _userMock.Object,
            _identityServiceMock.Object);

        _userMock.Setup(x => x.Id).Returns("test-user-id");
        _identityServiceMock.Setup(x => x.AuthorizeAsync("test-user-id", "CanEdit"))
            .ReturnsAsync(true);
_identityServiceMock.Setup(x => x.AuthorizeAsync("test-user-id", "CanDelete"))
   .ReturnsAsync(true);

        // Act
        await behaviour.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(x => x(), Times.Once);
     _identityServiceMock.Verify(x => x.AuthorizeAsync("test-user-id", "CanEdit"), Times.Once);
    _identityServiceMock.Verify(x => x.AuthorizeAsync("test-user-id", "CanDelete"), Times.Once);
    }

 [Fact]
    public async Task Handle_WhenOneOfMultiplePoliciesFails_ShouldThrowForbiddenAccessException()
    {
     // Arrange
        var request = new TestRequestWithMultiplePolicies();
    var behaviour = new AuthorizationBehaviour<TestRequestWithMultiplePolicies, Unit>(
          _userMock.Object,
            _identityServiceMock.Object);

        _userMock.Setup(x => x.Id).Returns("test-user-id");
        _identityServiceMock.Setup(x => x.AuthorizeAsync("test-user-id", "CanEdit"))
          .ReturnsAsync(true);
        _identityServiceMock.Setup(x => x.AuthorizeAsync("test-user-id", "CanDelete"))
            .ReturnsAsync(false);

 // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
        behaviour.Handle(request, _nextMock.Object, CancellationToken.None));

        _nextMock.Verify(x => x(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenBothRolesAndPoliciesAreRequired_AndBothPass_ShouldCallNext()
{
        // Arrange
        var request = new TestRequestWithRoleAndPolicy();
        var behaviour = new AuthorizationBehaviour<TestRequestWithRoleAndPolicy, Unit>(
     _userMock.Object,
  _identityServiceMock.Object);

        _userMock.Setup(x => x.Id).Returns("test-user-id");
     _userMock.Setup(x => x.Roles).Returns(new List<string> { "Admin" });
    _identityServiceMock.Setup(x => x.AuthorizeAsync("test-user-id", "CanEdit"))
 .ReturnsAsync(true);

        // Act
        await behaviour.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRolePassesButPolicyFails_ShouldThrowForbiddenAccessException()
    {
// Arrange
        var request = new TestRequestWithRoleAndPolicy();
 var behaviour = new AuthorizationBehaviour<TestRequestWithRoleAndPolicy, Unit>(
   _userMock.Object,
      _identityServiceMock.Object);

        _userMock.Setup(x => x.Id).Returns("test-user-id");
        _userMock.Setup(x => x.Roles).Returns(new List<string> { "Admin" });
  _identityServiceMock.Setup(x => x.AuthorizeAsync("test-user-id", "CanEdit"))
   .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
    behaviour.Handle(request, _nextMock.Object, CancellationToken.None));

        _nextMock.Verify(x => x(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRolesHaveWhitespace_ShouldTrimAndMatch()
    {
        // Arrange
        var request = new TestRequestWithRoleWithSpaces();
        var behaviour = new AuthorizationBehaviour<TestRequestWithRoleWithSpaces, Unit>(
  _userMock.Object,
       _identityServiceMock.Object);

 _userMock.Setup(x => x.Id).Returns("test-user-id");
        _userMock.Setup(x => x.Roles).Returns(new List<string> { "Admin" });

   // Act
   await behaviour.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(x => x(), Times.Once);
    }
}

// Test request classes
public class TestRequest { }

public class TestRequestWithoutAuth { }

[Authorize]
public class TestRequestWithEmptyAuthorize { }

[Authorize(Roles = "Admin")]
public class TestRequestWithRole { }

[Authorize(Roles = "Admin,Manager,User")]
public class TestRequestWithMultipleRoles { }

[Authorize(Policy = "CanEdit")]
public class TestRequestWithPolicy { }

[Authorize(Policy = "CanEdit")]
[Authorize(Policy = "CanDelete")]
public class TestRequestWithMultiplePolicies { }

[Authorize(Roles = "Admin", Policy = "CanEdit")]
public class TestRequestWithRoleAndPolicy { }

[Authorize(Roles = " Admin , Manager ")]
public class TestRequestWithRoleWithSpaces { }
