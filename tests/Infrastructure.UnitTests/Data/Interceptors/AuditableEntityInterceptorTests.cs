using FinalProject.Application.Common.Interfaces;
using FinalProject.Domain.Common;
using FinalProject.Domain.Entities;
using FinalProject.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace FinalProject.Infrastructure.UnitTests.Data.Interceptors;

public class AuditableEntityInterceptorTests
{
    private readonly Mock<IUser> _userMock;
    private readonly Mock<TimeProvider> _timeProviderMock;
    private readonly AuditableEntityInterceptor _interceptor;
    private readonly DateTimeOffset _testDateTime;

    public AuditableEntityInterceptorTests()
    {
        _userMock = new Mock<IUser>();
        _timeProviderMock = new Mock<TimeProvider>();
        _testDateTime = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);

        _userMock.Setup(x => x.Id).Returns("test-user-id");
        _timeProviderMock.Setup(x => x.GetUtcNow()).Returns(_testDateTime);

        _interceptor = new AuditableEntityInterceptor(_userMock.Object, _timeProviderMock.Object);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenEntityAdded_ShouldSetCreatedProperties()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
  .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new TestDbContext(options);
        var entity = new TodoItem { Title = "Test" };
        context.TodoItems.Add(entity);

        // Act
        _interceptor.UpdateEntities(context);

        // Assert
        Assert.Equal("test-user-id", entity.CreatedBy);
        Assert.Equal(_testDateTime, entity.Created);
        Assert.Equal("test-user-id", entity.LastModifiedBy);
        Assert.Equal(_testDateTime, entity.LastModified);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenEntityModified_ShouldUpdateLastModifiedProperties()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

        await using var context = new TestDbContext(options);
        var entity = new TodoItem
        {
            Title = "Test",
            CreatedBy = "original-user",
            Created = DateTimeOffset.UtcNow.AddDays(-1)
        };

        context.TodoItems.Add(entity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Modify entity
        entity.Title = "Modified";
        context.Entry(entity).State = EntityState.Modified;

        // Act
        _interceptor.UpdateEntities(context);

        // Assert
        Assert.Equal("original-user", entity.CreatedBy); // Should not change
        Assert.Equal("test-user-id", entity.LastModifiedBy);
        Assert.Equal(_testDateTime, entity.LastModified);
    }

    [Fact]
    public void UpdateEntities_WhenContextIsNull_ShouldNotThrowException()
    {
        // Act & Assert
        var exception = Record.Exception(() => _interceptor.UpdateEntities(null));
        Assert.Null(exception);
    }

    [Fact]
    public void UpdateEntities_WhenEntityAdded_ShouldSetCreatedProperties()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
       .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
     .Options;

        using var context = new TestDbContext(options);
        var entity = new TodoItem { Title = "Test" };
        context.TodoItems.Add(entity);

        // Act
        _interceptor.UpdateEntities(context);

        // Assert
        Assert.Equal("test-user-id", entity.CreatedBy);
        Assert.Equal(_testDateTime, entity.Created);
        Assert.Equal("test-user-id", entity.LastModifiedBy);
        Assert.Equal(_testDateTime, entity.LastModified);
    }
}

// Helper DbContext for testing
public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}
