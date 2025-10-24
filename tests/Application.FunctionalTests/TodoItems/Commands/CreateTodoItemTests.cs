using FinalProject.Application.Common.Exceptions;
using FinalProject.Application.TodoItems.Commands.CreateTodoItem;
using FinalProject.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace FinalProject.Application.FunctionalTests.TodoItems.Commands;


public class CreateTodoItemTests(Testing testing) : BaseTestFixture(testing)
{
    [Fact]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTodoItemCommand();
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task ShouldCreateTodoItem()
    {
        var command = new CreateTodoItemCommand
        {
            Title = "Test TodoItem"
        };

        var itemId = await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.Title.Should().Be(command.Title);
        item.Done.Should().BeFalse();
        item.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
