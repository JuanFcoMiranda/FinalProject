# ?? Documentación de Testing

Guías sobre pruebas, cobertura de código y calidad del software.

## ?? Documentos Disponibles

### ?? Cobertura de Código
- **[coverage-readme.md](coverage-readme.md)** - Introducción a la cobertura de tests
- **[coverage-overview.md](coverage-overview.md)** - Detalles completos sobre la cobertura actual
- **[coverage-summary.md](coverage-summary.md)** - Resumen ejecutivo de métricas

### ?? Guías
- **[run-coverage-guide.md](run-coverage-guide.md)** - Cómo ejecutar análisis de cobertura

## ?? Inicio Rápido

### Ejecutar Todos los Tests

```bash
# Ejecutar todos los tests
dotnet test

# Con logs detallados
dotnet test --logger "console;verbosity=detailed"

# Solo tests unitarios
dotnet test --filter "FullyQualifiedName~UnitTests"

# Solo tests funcionales
dotnet test --filter "FullyQualifiedName~FunctionalTests"
```

### Generar Reporte de Cobertura

```bash
# Instalar herramientas (primera vez)
dotnet tool install -g dotnet-coverage
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generar cobertura
dotnet test --collect:"XPlat Code Coverage"

# Generar reporte HTML
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html

# Abrir reporte
start coverage-report/index.html
```

## ?? Métricas Actuales

| Métrica | Valor | Estado |
|---------|-------|--------|
| **Cobertura Global** | ~85% | ? Excelente |
| **Tests Unitarios** | 150+ | ? |
| **Tests Funcionales** | 30+ | ? |
| **Tests de Integración** | 20+ | ? |

Ver detalles completos en [coverage-overview.md](coverage-overview.md)

## ??? Estructura de Tests

```
tests/
??? Domain.UnitTests/              # Tests de entidades de dominio
??? Application.UnitTests/         # Tests de casos de uso
??? Infrastructure.UnitTests/      # Tests de infraestructura
??? Web.UnitTests/                 # Tests de endpoints
??? Application.FunctionalTests/   # Tests end-to-end
```

## ?? Tipos de Tests

### Tests Unitarios
- **Propósito**: Probar componentes individuales aislados
- **Velocidad**: Muy rápida (~ms)
- **Frameworks**: xUnit, Moq, FluentAssertions, Shouldly
- **Ubicación**: `*.UnitTests`

```csharp
[Fact]
public void Should_Create_TodoItem_With_Valid_Title()
{
    // Arrange
    var command = new CreateTodoItemCommand { Title = "Test" };

    // Act
    var result = command.Title;

    // Assert
    result.Should().Be("Test");
}
```

### Tests Funcionales
- **Propósito**: Probar flujos completos end-to-end
- **Velocidad**: Más lenta (~segundos)
- **Frameworks**: xUnit, Testcontainers, WebApplicationFactory
- **Ubicación**: `Application.FunctionalTests`

```csharp
[Fact]
public async Task Should_Create_And_Retrieve_TodoItem()
{
    // Arrange
    var command = new CreateTodoItemCommand { Title = "Test" };

    // Act
    var id = await SendAsync(command);
    var item = await FindAsync<TodoItem>(id);

    // Assert
    item.Should().NotBeNull();
    item.Title.Should().Be("Test");
}
```

## ??? Herramientas de Testing

### Frameworks Principales
- **xUnit** - Framework de testing
- **Moq** - Mocking framework
- **FluentAssertions** - Assertions fluidas
- **Shouldly** - Assertions legibles
- **Testcontainers** - Contenedores para tests de integración

### Herramientas de Cobertura
- **coverlet** - Generación de cobertura
- **ReportGenerator** - Reportes HTML de cobertura
- **dotnet-coverage** - Herramienta de Microsoft

## ?? Objetivos de Cobertura

| Componente | Objetivo | Actual |
|------------|----------|--------|
| Domain | 90%+ | ? 92% |
| Application | 85%+ | ? 88% |
| Infrastructure | 70%+ | ? 75% |
| Web | 80%+ | ? 82% |

## ?? Mejores Prácticas

### Nomenclatura de Tests
```csharp
// Patrón: Should_ExpectedBehavior_When_Condition
[Fact]
public void Should_ReturnTrue_When_TodoItemIsCompleted()

// Patrón: Given_Precondition_When_Action_Then_Result
[Fact]
public void Given_ValidTodoItem_When_Completed_Then_EventIsRaised()
```

### Estructura AAA (Arrange-Act-Assert)
```csharp
[Fact]
public async Task CreateTodoItem_Test()
{
    // Arrange - Preparar datos y estado
    var command = new CreateTodoItemCommand { Title = "Test" };
    
    // Act - Ejecutar la acción
    var result = await SendAsync(command);
    
    // Assert - Verificar resultado
    result.Should().BeGreaterThan(0);
}
```

### Tests Descriptivos
```csharp
// ? Mal
[Fact]
public void Test1() { }

// ? Bien
[Fact]
public void Should_Throw_ValidationException_When_Title_Is_Empty()
{
    // Test implementation
}
```

## ?? Debugging Tests

### VS Code
```bash
# Ejecutar test específico con debugger
# 1. Poner breakpoint en el test
# 2. F5 o "Run and Debug"
```

### VS 2022
```bash
# Test Explorer
# 1. Abrir Test Explorer (Ctrl+E, T)
# 2. Click derecho ? Debug
```

### Línea de Comandos
```bash
# Con logs verbosos
dotnet test --logger "console;verbosity=detailed"

# Test específico
dotnet test --filter "FullyQualifiedName~CreateTodoItemTests.Should_Create_TodoItem"
```

## ?? Reportes de Cobertura

### Generar Reporte Completo
Ver guía detallada en [run-coverage-guide.md](run-coverage-guide.md)

### Leer Reportes
- **Líneas verdes**: Código cubierto por tests
- **Líneas rojas**: Código sin cobertura
- **Líneas amarillas**: Cobertura parcial (ej: if sin else)

### Métricas Importantes
- **Line Coverage**: % de líneas ejecutadas
- **Branch Coverage**: % de ramas ejecutadas
- **Method Coverage**: % de métodos ejecutados

## ?? CI/CD Integration

### GitHub Actions
```yaml
- name: Run tests
  run: dotnet test --collect:"XPlat Code Coverage"

- name: Generate coverage report
  run: reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage

- name: Upload coverage
  uses: codecov/codecov-action@v3
```

## ?? Recursos de Aprendizaje

### Internos
- [Coverage Overview](coverage-overview.md)
- [Run Coverage Guide](run-coverage-guide.md)

### Externos
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [Test Driven Development](https://martinfowler.com/bliki/TestDrivenDevelopment.html)

## ?? Enlaces Relacionados

- [Documentación de Docker](../docker/)
- [Documentación de Base de Datos](../database/)
- [README Principal](../README.md)

---

**Última actualización**: Enero 2025
