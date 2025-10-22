using FinalProject.Domain.Constants;
using FinalProject.Infrastructure.Data;
using FinalProject.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinalProject.Application.FunctionalTests;

public class Testing : IAsyncLifetime
{
    private ITestDatabase _database = null!;
    private CustomWebApplicationFactory _factory = null!;
    private IServiceScopeFactory _scopeFactory = null!;
    private string? _userId;
    private List<string>? _roles;

    public async ValueTask InitializeAsync()
    {
        _database = await TestDatabaseFactory.CreateAsync();

        _factory = new CustomWebApplicationFactory(
            _database.GetConnection(), 
            _database.GetConnectionString(),
            () => _userId,
            () => _roles);

        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public async Task SendAsync(IBaseRequest request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        await mediator.Send(request);
    }

    public string? GetUserId()
    {
        return _userId;
    }
    
    public List<string>? GetRoles()
    {
        return _roles;
    }

    public async Task<string> RunAsDefaultUserAsync()
    {
        return await RunAsUserAsync("test@local", "Testing1234!", []);
    }

    public async Task<string> RunAsAdministratorAsync()
    {
        return await RunAsUserAsync("administrator@local", "Administrator1234!", [Roles.Administrator]);
    }

    public async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
    {
        using var scope = _scopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser { UserName = userName, Email = userName };

        var result = await userManager.CreateAsync(user, password);

        if (roles.Any())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            await userManager.AddToRolesAsync(user, roles);
        }

        if (result.Succeeded)
        {
            _userId = user.Id;
            _roles = roles.ToList();
            return _userId;
        }

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }

    public async Task ResetState()
    {
        try
        {
            await _database.ResetAsync();
        }
        catch (Exception)
        {
            // Ignore exceptions during reset
        }

        _userId = null;
    }

    public async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _database.DisposeAsync();
        await _factory.DisposeAsync();
    }
}

[CollectionDefinition(nameof(TestingCollection))]
public class TestingCollection : ICollectionFixture<Testing>
{
}
