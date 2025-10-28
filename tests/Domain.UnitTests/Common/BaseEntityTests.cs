using FinalProject.Domain.Common;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Events;
using Xunit;

namespace FinalProject.Domain.UnitTests.Common;

public class BaseEntityTests
{
    [Fact]
    public void ShouldAddDomainEvent()
{
        // Arrange
  var entity = new TodoItem { Title = "Test" };
        var domainEvent = new TodoItemCreatedEvent(entity);

     // Act
     entity.AddDomainEvent(domainEvent);

   // Assert
        Assert.Single(entity.DomainEvents);
        Assert.Contains(domainEvent, entity.DomainEvents);
    }

    [Fact]
    public void ShouldRemoveDomainEvent()
{
        // Arrange
   var entity = new TodoItem { Title = "Test" };
    var domainEvent = new TodoItemCreatedEvent(entity);
        entity.AddDomainEvent(domainEvent);

  // Act
        entity.RemoveDomainEvent(domainEvent);

     // Assert
        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public void ShouldClearDomainEvents()
    {
        // Arrange
        var entity = new TodoItem { Title = "Test" };
        entity.AddDomainEvent(new TodoItemCreatedEvent(entity));
        entity.AddDomainEvent(new TodoItemCompletedEvent(entity));

   // Act
        entity.ClearDomainEvents();

        // Assert
        Assert.Empty(entity.DomainEvents);
    }

  [Fact]
    public void ShouldReturnReadOnlyCollectionOfDomainEvents()
    {
      // Arrange
        var entity = new TodoItem { Title = "Test" };
 var domainEvent = new TodoItemCreatedEvent(entity);
    entity.AddDomainEvent(domainEvent);

        // Act
        var events = entity.DomainEvents;

 // Assert
        Assert.IsAssignableFrom<IReadOnlyCollection<BaseEvent>>(events);
    }
}
