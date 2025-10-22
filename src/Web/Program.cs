using FastEndpoints;
using FinalProject.Application;
using FinalProject.Infrastructure.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddInfrastructureServices();
builder.AddWebServices();

builder.Services.AddApplicationServices();
builder.Services.AddFastEndpoints();
builder.Services.AddOpenApi();

var app = builder.Build();

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
