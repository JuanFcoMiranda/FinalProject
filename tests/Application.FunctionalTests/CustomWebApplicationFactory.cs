using System.Data.Common;
using FinalProject.Application.Common.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FinalProject.Application.FunctionalTests;

public class CustomWebApplicationFactory(
    DbConnection connection,
    string connectionString,
    Func<string?> getUserId,
    Func<List<string>?> getRoles)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("Testing")
            .UseSetting("ConnectionStrings:FinalProjectDb", connectionString);

        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IUser>()
                .AddTransient(provider =>
                {
                    var mock = new Mock<IUser>();
                    mock.SetupGet(x => x.Roles).Returns(getRoles());
                    mock.SetupGet(x => x.Id).Returns(getUserId());
                    return mock.Object;
                });
            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                    options.UseSqlServer(connection);
                });
        });
    }
}
