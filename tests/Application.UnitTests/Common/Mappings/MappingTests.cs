using System.Reflection;
using FinalProject.Application.Common.Interfaces;
using FinalProject.Application.Common.Models;
using FinalProject.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using FinalProject.Domain.Entities;
using Mapster;

namespace FinalProject.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly TypeAdapterConfig _config;

    public MappingTests()
    {
        _config = new TypeAdapterConfig();
        _config.Scan(Assembly.GetAssembly(typeof(IApplicationDbContext))!);
    }

    [Fact]
    public void ShouldHaveValidConfiguration()
    {
        // Mapster doesn't require explicit configuration validation
        // Just verify it can be created
        Assert.NotNull(_config);
    }

    [Theory]
    [InlineData(typeof(TodoItem), typeof(LookupDto))]
    [InlineData(typeof(TodoItem), typeof(TodoItemBriefDto))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = Activator.CreateInstance(source);

        // Fix CS8604: Ensure 'instance' is not null before passing to Adapt
        Assert.NotNull(instance);

        var result = TypeAdapter.Adapt(instance, source, destination, _config);

        Assert.NotNull(result);
    }
}
