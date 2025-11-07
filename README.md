# FinalProject

The project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture) version 9.0.12.

## 📚 Documentación

Este proyecto incluye documentación completa organizada por temáticas:

- **[📖 Índice de Documentación](docs/README.md)** - Índice principal de toda la documentación
- **[🐳 Docker](docs/docker/README.md)** - Guías de Docker y contenedores
- **[💾 Base de Datos](docs/database/README.md)** - Configuración y migración de SQL Server
- **[🧪 Testing](docs/testing/README.md)** - Pruebas y cobertura de código

### 🚀 Inicio Rápido

**Opción 1: Docker (Recomendado)**
```bash
# Levantar todo el stack
docker-compose up -d

# Esperar ~20 segundos y verificar
curl http://localhost:8080/health
```
Ver más en [Docker Quickstart](docs/docker/quickstart.md)

**Opción 2: Desarrollo Local**
```bash
cd .\src\Web\
dotnet watch run
```

## Build

Run `dotnet build -tl` to build the solution.

## Run

To run the web application:

```bash
cd .\src\Web\
dotnet watch run
```

Navigate to https://localhost:5001. The application will automatically reload if you change any of the source files.

### 🐳 Con Docker

```bash
# Levantar servicios
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener servicios
docker-compose down
```

La aplicación estará disponible en http://localhost:8080

Ver guía completa en [docs/docker/](docs/docker/)

## Code Styles & Formatting

The template includes [EditorConfig](https://editorconfig.org/) support to help maintain consistent coding styles for multiple developers working on the same project across various editors and IDEs. The **.editorconfig** file defines the coding styles applicable to this solution.

## Code Scaffolding

The template includes support to scaffold new commands and queries.

Start in the `.\src\Application\` folder.

Create a new command:

```
dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int
```

Create a new query:

```
dotnet new ca-usecase -n GetTodos -fn TodoLists -ut query -rt TodosVm
```

If you encounter the error *"No templates or subcommands found matching: 'ca-usecase'."*, install the template and try again:

```bash
dotnet new install Clean.Architecture.Solution.Template::9.0.12
```

## Test

The solution contains unit, integration, and functional tests.

To run the tests:
```bash
dotnet test
```

### 📊 Con Cobertura

```bash
# Ejecutar tests con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Generar reporte HTML
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html

# Abrir reporte
start coverage-report/index.html
```

Ver guía completa en [docs/testing/](docs/testing/)

## 🗄️ Base de Datos

El proyecto usa SQL Server y Entity Framework Core.

**Migrar Base de Datos:**
```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

**Crear Nueva Migración:**
```bash
dotnet ef migrations add NombreMigracion --project src/Infrastructure --startup-project src/Web
```

Ver guía completa en [docs/database/](docs/database/)

## 🐛 Solución de Problemas

### Error de contenedor SQL Server
→ Ver [docs/docker/database-container-fix.md](docs/docker/database-container-fix.md)

### Error "Cannot open database"
→ Ver [docs/database/connection-fix.md](docs/database/connection-fix.md)

### Problemas con Docker
→ Ver [docs/docker/error-solution.md](docs/docker/error-solution.md)

## 📖 Recursos

- **[Documentación Completa](docs/)** - Toda la documentación del proyecto
- **[Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)** - Template original
- **[ASP.NET Core](https://docs.microsoft.com/aspnet/core)** - Framework web
- **[Entity Framework Core](https://docs.microsoft.com/ef/core)** - ORM
- **[FastEndpoints](https://fast-endpoints.com/)** - API endpoints

## Help

To learn more about the template go to the [project website](https://github.com/jasontaylordev/CleanArchitecture). Here you can find additional guidance, request new features, report a bug, and discuss the template with other users.