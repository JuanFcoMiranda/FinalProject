using FinalProject.Application.TodoItems.Commands.CreateTodoItem;
using FinalProject.Application.TodoItems.Commands.UpdateTodoItemDetail;
using FinalProject.Domain.Entities;
using FinalProject.Domain.Enums;
using FluentAssertions;

namespace FinalProject.Application.FunctionalTests.TodoItems.Commands;

public class UpdateTodoItemDetailTests(Testing testing) : BaseTestFixture(testing)
{
    [Fact]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemDetailCommand { Id = 99, Note = "Note" };
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ShouldUpdateTodoItem()
    {
        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            Title = "Test Item"
        });

        var command = new UpdateTodoItemDetailCommand
        {
            Id = itemId,
            Priority = PriorityLevel.High,
            Note = "This is a test note"
        };

        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.Priority.Should().Be(command.Priority);
        item.Note.Should().Be(command.Note);
        //item.LastModified.ShouldNotBe((DateTimeOffset)null);
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
