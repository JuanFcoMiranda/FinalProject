using FinalProject.Application.TodoItems.Commands.CreateTodoItem;
using FinalProject.Application.TodoItems.Commands.DeleteTodoItem;
using FinalProject.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace FinalProject.Application.FunctionalTests.TodoItems.Commands;

public class DeleteTodoItemTests(Testing testing) : BaseTestFixture(testing)
{
    [Fact]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new DeleteTodoItemCommand(99);
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ShouldDeleteTodoItem()
    {
        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            Title = "Test Item"
        });

        await SendAsync(new DeleteTodoItemCommand(itemId));

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().BeNull();
    }
}
