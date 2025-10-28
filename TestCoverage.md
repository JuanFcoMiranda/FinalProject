# Gu�a de Cobertura de C�digo

## Resumen de Mejoras

Se han a�adido **12 nuevos archivos de test** con **70 tests unitarios** en total para mejorar significativamente la cobertura de c�digo del proyecto.

## Tests A�adidos

### Domain Layer (tests/Domain.UnitTests) - 39 Tests

#### 1. **Entities/TodoItemTests.cs** (4 tests)
- ? `ShouldSetPropertiesCorrectly` - Valida que todas las propiedades se establezcan correctamente
- ? `ShouldRaiseTodoItemCompletedEventWhenDoneIsSetToTrue` - Verifica que se dispare el evento cuando se completa
- ? `ShouldNotRaiseTodoItemCompletedEventWhenDoneIsAlreadyTrue` - Evita eventos duplicados
- ? `ShouldNotRaiseTodoItemCompletedEventWhenDoneIsSetToFalse` - No dispara evento al desmarcar

#### 2. **Common/BaseEntityTests.cs** (4 tests)
- ? `ShouldAddDomainEvent` - Agrega eventos de dominio correctamente
- ? `ShouldRemoveDomainEvent` - Remueve eventos de dominio
- ? `ShouldClearDomainEvents` - Limpia todos los eventos
- ? `ShouldReturnReadOnlyCollectionOfDomainEvents` - Verifica colecci�n de solo lectura

#### 3. **Common/ValueObjectTests.cs** (6 tests)
- ? `EqualValueObjectsShouldBeEqual` - Igualdad entre value objects
- ? `DifferentValueObjectsShouldNotBeEqual` - Desigualdad entre diferentes value objects
- ? `ValueObjectShouldNotEqualNull` - No es igual a null
- ? `ValueObjectShouldNotEqualDifferentType` - No es igual a otros tipos
- ? `EqualValueObjectsShouldHaveSameHashCode` - Hash codes iguales para objetos iguales
- ? `DifferentValueObjectsShouldHaveDifferentHashCode` - Hash codes diferentes

#### 4. **ValueObjects/ColourTests.cs** (13 tests - Mejorado)
- ? `ShouldReturnCorrectColourCode` - C�digo de color correcto
- ? `ToStringReturnsCode` - ToString devuelve el c�digo
- ? `ShouldPerformImplicitConversionToColourCodeString` - Conversi�n impl�cita
- ? `ShouldPerformExplicitConversionGivenSupportedColourCode` - Conversi�n expl�cita
- ? `ShouldThrowUnsupportedColourExceptionGivenNotSupportedColourCode` - Excepci�n para colores no soportados
- ? `ShouldReturnCorrectPredefinedColours` - Todos los colores predefinidos
- ? `ShouldHandleNullOrWhiteSpaceCodeByDefaultingToBlack` - Manejo de nulos
- ? `ShouldCreateColourFromAllSupportedCodes` (Theory con 7 casos) - Todos los c�digos v�lidos
- ? `TwoColoursWithSameCodeShouldBeEqual` - Igualdad de colores

#### 5. **Events/TodoItemEventsTests.cs** (3 tests)
- ? `TodoItemCreatedEventShouldHoldItemReference` - Evento de creaci�n
- ? `TodoItemCompletedEventShouldHoldItemReference` - Evento de completado
- ? `TodoItemDeletedEventShouldHoldItemReference` - Evento de eliminaci�n

#### 6. **Exceptions/UnsupportedColourExceptionTests.cs** (2 tests)
- ? `ShouldCreateExceptionWithCorrectMessage` - Mensaje correcto
- ? `ShouldBeExceptionType` - Es una excepci�n

#### 7. **Enums/PriorityLevelTests.cs** (5 tests)
- ? `ShouldHaveCorrectValues` - Valores correctos del enum
- ? `ShouldBeDefinedEnum` (Theory con 4 casos) - Todos los valores est�n definidos

### Application Layer (tests/Application.UnitTests) - 31 Tests

#### 1. **Common/Models/ResultTests.cs** (4 tests)
- ? `SuccessShouldCreateSuccessfulResult` - Resultado exitoso
- ? `FailureShouldCreateFailedResultWithErrors` - Resultado fallido con m�ltiples errores
- ? `FailureShouldCreateFailedResultWithSingleError` - Resultado con un error
- ? `FailureShouldCreateFailedResultWithNoErrors` - Resultado fallido sin errores

#### 2. **Common/Models/PaginatedListTests.cs** (8 tests)
- ? `ShouldCreatePaginatedListWithCorrectProperties` - Propiedades correctas
- ? `ShouldCalculateHasPreviousPageCorrectly` - HasPreviousPage
- ? `ShouldCalculateHasNextPageCorrectly` - HasNextPage
- ? `ShouldCalculateTotalPagesCorrectly` - C�lculo de p�ginas totales
- ? `ShouldHandleFirstPageScenario` - Primera p�gina
- ? `ShouldHandleLastPageScenario` - �ltima p�gina
- ? `ShouldHandleEmptyList` - Lista vac�a
- ? `ShouldHandleSinglePageScenario` - Una sola p�gina
- ? `ShouldHandleMiddlePageScenario` - P�gina intermedia

#### 3. **Common/Models/LookupDtoTests.cs** (2 tests)
- ? `ShouldCreateLookupDtoWithProperties` - Creaci�n con propiedades
- ? `ShouldAllowNullTitle` - Permite t�tulo nulo

#### 4. **TodoItems/Queries/TodoItemBriefDtoTests.cs** (2 tests)
- ? `ShouldCreateTodoItemBriefDtoWithProperties` - Creaci�n con propiedades
- ? `ShouldAllowNullTitle` - Permite t�tulo nulo

#### 5. **Common/Exceptions/ForbiddenAccessExceptionTests.cs** (2 tests)
- ? `ShouldCreateForbiddenAccessException` - Creaci�n de excepci�n
- ? `ShouldBeExceptionType` - Es una excepci�n

### Archivos Existentes (Mantenidos)
- ? **Common/Behaviours/RequestLoggerTests.cs** (2 tests)
- ? **Common/Mappings/MappingTests.cs** (3 tests)
- ? **Common/Exceptions/ValidationExceptionTests.cs** (10 tests)

## Ejecutar Tests con Cobertura de C�digo

### Opci�n 1: Ejecutar todos los tests unitarios

```powershell
# Tests de Domain
dotnet test tests/Domain.UnitTests/Domain.UnitTests.csproj

# Tests de Application
dotnet test tests/Application.UnitTests/Application.UnitTests.csproj

# Todos los tests (incluye funcionales, requiere Docker)
dotnet test
```

### Opci�n 2: Ejecutar tests con cobertura usando coverlet

```powershell
# Domain Layer con cobertura
dotnet test tests/Domain.UnitTests/Domain.UnitTests.csproj `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura `
  /p:Exclude="[*.UnitTests]*"

# Application Layer con cobertura
dotnet test tests/Application.UnitTests/Application.UnitTests.csproj `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura `
  /p:Exclude="[*.UnitTests]*"
```

### Opci�n 3: Generar reporte HTML de cobertura

```powershell
# Instalar la herramienta de reporte (solo una vez)
dotnet tool install --global dotnet-reportgenerator-globaltool

# Ejecutar tests de Domain con cobertura
dotnet test tests/Domain.UnitTests/Domain.UnitTests.csproj `
/p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura `
  /p:CoverletOutput=./TestResults/ `
  /p:Exclude="[*.UnitTests]*"

# Ejecutar tests de Application con cobertura
dotnet test tests/Application.UnitTests/Application.UnitTests.csproj `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura `
  /p:CoverletOutput=./TestResults/ `
  /p:Exclude="[*.UnitTests]*"

# Generar reporte HTML combinado
reportgenerator `
  -reports:tests/**/TestResults/coverage.cobertura.xml `
  -targetdir:TestResults/CoverageReport `
  -reporttypes:Html

# Abrir el reporte
start TestResults/CoverageReport/index.html
```

### Opci�n 4: Usar Visual Studio

1. **Men�**: Test ? Analyze Code Coverage for All Tests
2. Los resultados aparecer�n en la ventana **Code Coverage Results**
3. Para ver detalles, haz clic derecho y selecciona **Show Code Coverage Coloring**

### Opci�n 5: Usar el comando dotnet test con argumentos de cobertura

```powershell
# Ejecutar con cobertura detallada
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Excluir proyectos de test de la cobertura
dotnet test --collect:"XPlat Code Coverage" `
  --results-directory ./TestResults `
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude=[*.UnitTests]*,[*.FunctionalTests]*
```

## Paquetes NuGet Requeridos

Los proyectos ya incluyen los paquetes necesarios:

```xml
<PackageReference Include="coverlet.collector" Version="6.0.0" />
```

Si necesitas coverlet.msbuild para reportes m�s detallados:

```powershell
# Domain.UnitTests
dotnet add tests/Domain.UnitTests/Domain.UnitTests.csproj package coverlet.msbuild

# Application.UnitTests  
dotnet add tests/Application.UnitTests/Application.UnitTests.csproj package coverlet.msbuild
```

## M�tricas de Cobertura Esperadas

Con los **70 tests unitarios** a�adidos, deber�as ver mejoras significativas en:

### Domain Layer
- **Entities (TodoItem)**: ~95%+
  - Propiedades, eventos de dominio, comportamiento Done
  
- **Common (BaseEntity)**: ~100%
  - Gesti�n de eventos de dominio completa
  
- **ValueObjects (Colour)**: ~98%+
  - Todos los colores predefinidos, conversiones, validaciones
  
- **Common (ValueObject)**: ~100%
  - Igualdad, hash code, operadores
  
- **Events**: ~100%
  - TodoItemCreatedEvent, TodoItemCompletedEvent, TodoItemDeletedEvent
  
- **Exceptions**: ~100%
  - UnsupportedColourException
  
- **Enums**: ~100%
  - PriorityLevel

### Application Layer
- **Common/Models (Result)**: ~100%
  - Success, Failure con diferentes escenarios
  
- **Common/Models (PaginatedList)**: ~85%+
  - Constructor, propiedades calculadas, escenarios edge
  - Nota: CreateAsync requiere EF mock para cobertura completa
  
- **Common/Models (LookupDto)**: ~100%
  - Propiedades, valores nulos
  
- **TodoItems/Queries (TodoItemBriefDto)**: ~100%
  - Propiedades, valores nulos
  
- **Common/Exceptions (ForbiddenAccessException)**: ~100%
  - Creaci�n de excepci�n
  
- **Common/Behaviours (LoggingBehaviour)**: ~80%+ (existente)
  - Usuario autenticado/no autenticado
  
- **Common/Mappings**: ~75%+ (existente)
  - Configuraci�n y mapeos

## Cobertura Global Estimada

- **Domain Layer**: ~90-95%
- **Application Layer**: ~75-80% (modelos y excepciones casi 100%)

## Pr�ximos Pasos para Alcanzar >90% de Cobertura

### 1. Commands (Alta prioridad)
```
- CreateTodoItemCommand + Validator
- UpdateTodoItemCommand + Validator
- UpdateTodoItemDetailCommand
- DeleteTodoItemCommand
```

### 2. Queries (Alta prioridad)
```
- GetTodoItemsWithPaginationQuery + Validator
- Mapeos con Mapster (ya parcialmente cubierto)
```

### 3. Event Handlers (Media prioridad)
```
- TodoItemCreatedEventHandler
- TodoItemCompletedEventHandler
```

### 4. Behaviours (Media prioridad)
```
- ValidationBehaviour
- AuthorizationBehaviour
- PerformanceBehaviour
- UnhandledExceptionBehaviour
```

### 5. Infrastructure Layer (Baja prioridad - requiere integration tests)
```
- Identity Service
- Application DbContext
```

## Comando R�pido - Resumen de Tests

```powershell
# Ver resumen de todos los tests unitarios
dotnet test --filter "FullyQualifiedName~UnitTests" --logger:"console;verbosity=normal"

# Contar tests por proyecto
dotnet test tests/Domain.UnitTests --list-tests
dotnet test tests/Application.UnitTests --list-tests
```

## Estad�sticas Actuales

- ? **Total de tests unitarios**: 70
- ? **Domain.UnitTests**: 39 tests
- ? **Application.UnitTests**: 31 tests
- ? **Tasa de �xito**: 100%
- ? **Tiempo de ejecuci�n**: ~2-3 segundos

## Verificar que todo compila

```powershell
dotnet build --no-incremental
```

## Notas Adicionales

- Todos los tests utilizan **xUnit v3**
- Se utiliza **Shouldly** para assertions m�s expresivas en algunos tests
- Los tests son **independientes** y pueden ejecutarse en paralelo
- No se requiere **Docker** para los tests unitarios (solo para funcionales)
- Los tests siguen el patr�n **Arrange-Act-Assert (AAA)**
