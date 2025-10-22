using System.Reflection;
using FinalProject.Application.Common.Behaviours;
using Microsoft.Extensions.DependencyInjection;

namespace FinalProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        // Register MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        });

        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
