namespace FinalProject.Application.FunctionalTests;

[Collection(nameof(TestingCollection))]
public abstract class BaseTestFixture(Testing testing) : IAsyncLifetime
{
    protected readonly Testing Testing = testing;

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await Testing.ResetState();
    }

    protected Task<TResponse> SendAsync<TResponse>(MediatR.IRequest<TResponse> request) => Testing.SendAsync(request);
    protected Task SendAsync(MediatR.IBaseRequest request) => Testing.SendAsync(request);
    protected Task<string> RunAsDefaultUserAsync() => Testing.RunAsDefaultUserAsync();
    protected Task<string> RunAsAdministratorAsync() => Testing.RunAsAdministratorAsync();
    protected Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class => Testing.FindAsync<TEntity>(keyValues);
    protected Task AddAsync<TEntity>(TEntity entity) where TEntity : class => Testing.AddAsync(entity);
    protected Task<int> CountAsync<TEntity>() where TEntity : class => Testing.CountAsync<TEntity>();
}
