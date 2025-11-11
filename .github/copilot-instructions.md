# Copilot Instructions for FinalProject

## Architecture Overview
This is a .NET 9 Clean Architecture application using FastEndpoints, MediatR CQRS patterns, Entity Framework Core, and SQL Server. The solution follows strict separation of concerns across four layers:

- **Domain**: Core entities, value objects, enums, and domain events
- **Application**: Use cases (commands/queries), interfaces, behaviors, and DTOs  
- **Infrastructure**: Data access, external services, and cross-cutting concerns
- **Web**: FastEndpoints API layer with minimal controllers

## Key Patterns & Conventions

### CQRS with MediatR
Every use case is implemented as a command or query following this structure:
```csharp
// Commands: src/Application/{Feature}/Commands/{UseCase}/{UseCase}.cs
public record CreateTodoItemCommand : IRequest<int> { /* properties */ }
public class CreateTodoItemCommandHandler(IApplicationDbContext context) : IRequestHandler<...>

// Queries: src/Application/{Feature}/Queries/{UseCase}/{UseCase}.cs  
public record GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
```

### FastEndpoints Pattern
All API endpoints follow this structure in `src/Web/Endpoints/{Feature}/`:
```csharp
public class CreateTodoItemEndpoint(ISender sender) : Endpoint<CreateTodoItemCommand, int>
{
    public override void Configure()
    {
        Post("/api/TodoItems");
        AllowAnonymous(); // Change to RequireAuthorization() when auth is enabled
    }

    public override async Task HandleAsync(CreateTodoItemCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        Response = id;
        HttpContext.Response.StatusCode = 201;
    }
}
```

### Domain Events
Entities raise domain events which are handled by event handlers:
```csharp
// In entity: AddDomainEvent(new TodoItemCreatedEvent(this));
// Handler: src/Application/{Feature}/EventHandlers/
```

### Global Dependencies
Key global usings in Application layer: `Ardalis.GuardClauses`, `FluentValidation`, `Mapster`, `MediatR`, `EntityFrameworkCore`

## Development Workflows

### Adding New Features
1. Create command/query in `src/Application/{Feature}/{Commands|Queries}/{UseCase}/`
2. Add validator class with same naming pattern + `Validator` suffix
3. Create FastEndpoint in `src/Web/Endpoints/{Feature}/{UseCase}Endpoint.cs`
4. Use scaffolding: `dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int`

### Testing Strategy
- **Unit Tests**: Test individual handlers and validators
- **Functional Tests**: Inherit from `BaseTestFixture`, use `Testing` class for database operations
- **Test Database**: Uses Testcontainers for SQL Server integration tests
- Key test utilities: `SendAsync<T>()`, `FindAsync<T>()`, `AddAsync<T>()`, `RunAsDefaultUserAsync()`

### Build & Run Commands
```bash
# Build with terminal logger
dotnet build -tl

# Run with hot reload
cd .\src\Web\
dotnet watch run

# Run all tests
dotnet test
```

## Authentication Notes
Currently uses `AllowAnonymous()` on all endpoints - change to `RequireAuthorization()` when implementing auth. Identity system is configured but not enforced.

## Database & Migrations
- Entity Framework Core with SQL Server
- Connection string: "FinalProjectDb" 
- Database initializer runs automatically in development
- Audit interceptors for tracking entity changes

## CORS Configuration
Configured for Angular frontend on `http://localhost:4200` and `https://localhost:4200` with credentials support.

## API Documentation
Uses Scalar for API documentation at `/scalar/v1` (development only). OpenAPI spec available at `/openapi/v1.json`.