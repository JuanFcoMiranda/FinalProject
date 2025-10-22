using FinalProject.Application.TodoItems.Commands.CreateTodoItem;
using FinalProject.Application.TodoItems.Commands.UpdateTodoItem;
using FinalProject.Domain.Entities;
using FluentAssertions;

namespace FinalProject.Application.FunctionalTests.TodoItems.Commands;

public class UpdateTodoItemTests(Testing testing) : BaseTestFixture(testing)
{
    [Fact]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemCommand { Id = 99, Title = "New Title" };
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ShouldUpdateTodoItem()
    {
        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            Title = "Original Title"
        });

        var command = new UpdateTodoItemCommand
        {
            Id = itemId,
            Title = "Updated Title",
            Done = true
        };

        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.Title.Should().Be(command.Title);
        item.Done.Should().BeTrue();
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
