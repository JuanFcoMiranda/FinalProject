using Xunit;

namespace FinalProject.Application.FunctionalTests;

[Collection(nameof(TestingCollection))]
public abstract class BaseTestFixture(Testing testing) : IAsyncLifetime
{
    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await testing.ResetState();
    }

    protected Task<TResponse> SendAsync<TResponse>(MediatR.IRequest<TResponse> request) => testing.SendAsync(request);
    protected Task SendAsync(MediatR.IBaseRequest request) => testing.SendAsync(request);
    protected Task<string> RunAsDefaultUserAsync() => testing.RunAsDefaultUserAsync();
    protected Task<string> RunAsAdministratorAsync() => testing.RunAsAdministratorAsync();
    protected Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class => testing.FindAsync<TEntity>(keyValues);
    protected Task AddAsync<TEntity>(TEntity entity) where TEntity : class => testing.AddAsync(entity);
    protected Task<int> CountAsync<TEntity>() where TEntity : class => testing.CountAsync<TEntity>();
}
