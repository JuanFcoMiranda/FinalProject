using FastEndpoints;
using FinalProject.Application;
using FinalProject.Infrastructure.Data;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddInfrastructureServices();
builder.AddWebServices();

builder.Services.AddApplicationServices();
builder.Services.AddFastEndpoints();
builder.Services.AddOpenApi();

// --- OpenTelemetry configuration ---
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("FinalProject.Web"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation()
    )
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("FinalProject.Web"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter()
    );

var app = builder.Build();

// Endpoint Prometheus
app.MapPrometheusScrapingEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();

    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("FinalProject API")
              .WithDarkMode(true);
    });
}
else
{
    // En producción, ejecutar migraciones si la variable de entorno está configurada
    var runMigrations = builder.Configuration.GetValue("RUN_MIGRATIONS", false);
    if (runMigrations)
    {
        app.Logger.LogInformation("RUN_MIGRATIONS is enabled. Running database initialization...");

        try
        {
            await app.InitialiseDatabaseAsync();
            app.Logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while initializing the database. The application will continue to start, but database operations may fail.");
            // No lanzamos la excepción para que el contenedor no se detenga inmediatamente
            // Esto permite ver los logs y diagnosticar el problema
        }
    }
    else
    {
        app.Logger.LogInformation("RUN_MIGRATIONS is disabled. Skipping database initialization.");
    }

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable CORS - debe ir antes de UseAuthorization
app.UseCors(policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials());

app.UseExceptionHandler(options => { });

app.UseAuthorization();

app.UseFastEndpoints();

// MapFallbackToFile debe ir AL FINAL para que no intercepte las rutas de API y Scalar
app.MapFallbackToFile("index.html");

await app.RunAsync();

public partial class Program { }
