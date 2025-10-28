using FinalProject.Domain.Entities;
using FinalProject.Domain.Events;
using FinalProject.Infrastructure.Data.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace FinalProject.Infrastructure.UnitTests.Data.Interceptors;

public class DispatchDomainEventsInterceptorTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DispatchDomainEventsInterceptor _interceptor;

    public DispatchDomainEventsInterceptorTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _interceptor = new DispatchDomainEventsInterceptor(_mediatorMock.Object);
    }

    [Fact]
    public async Task DispatchDomainEvents_WhenEntityHasDomainEvents_ShouldPublishEvents()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

        using var context = new TestDbContext(options);
        var entity = new TodoItem { Title = "Test" };
        var domainEvent = new TodoItemCreatedEvent(entity);
        entity.AddDomainEvent(domainEvent);

        context.TodoItems.Add(entity);

        // Act
        await _interceptor.DispatchDomainEvents(context);

        // Assert
        _mediatorMock.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public async Task DispatchDomainEvents_WhenMultipleEventsExist_ShouldPublishAllEvents()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
              .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

        using var context = new TestDbContext(options);
        var entity = new TodoItem { Title = "Test" };

        var createdEvent = new TodoItemCreatedEvent(entity);
        var deletedEvent = new TodoItemDeletedEvent(entity);

        entity.AddDomainEvent(createdEvent);
        entity.AddDomainEvent(deletedEvent);

        context.TodoItems.Add(entity);

        // Act
        await _interceptor.DispatchDomainEvents(context);

        // Assert
        _mediatorMock.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public async Task DispatchDomainEvents_WhenContextIsNull_ShouldNotThrowException()
    {
        // Act & Assert
        var exception = await Record.ExceptionAsync(async () =>
            await _interceptor.DispatchDomainEvents(null));

        Assert.Null(exception);
        _mediatorMock.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DispatchDomainEvents_WhenNoEntitiesWithEvents_ShouldNotPublishEvents()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
             .Options;

        using var context = new TestDbContext(options);
        var entity = new TodoItem { Title = "Test" };
        // No domain events added

        context.TodoItems.Add(entity);

        // Act
        await _interceptor.DispatchDomainEvents(context);

        // Assert
        _mediatorMock.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DispatchDomainEvents_WhenContextHasMultipleEntities_ShouldPublishAllEvents()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
             .Options;

        using var context = new TestDbContext(options);

        var entity1 = new TodoItem { Title = "Test1" };
        var event1 = new TodoItemCreatedEvent(entity1);
        entity1.AddDomainEvent(event1);

        var entity2 = new TodoItem { Title = "Test2" };
        var event2 = new TodoItemCreatedEvent(entity2);
        entity2.AddDomainEvent(event2);

        context.TodoItems.Add(entity1);
        context.TodoItems.Add(entity2);

        // Act
        await _interceptor.DispatchDomainEvents(context);

        // Assert
        _mediatorMock.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        Assert.Empty(entity1.DomainEvents);
        Assert.Empty(entity2.DomainEvents);
    }
}
