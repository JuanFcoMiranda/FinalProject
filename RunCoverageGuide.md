# ?? Script de Verificación de Cobertura

## Comandos Rápidos

### 1. Ejecutar Tests Unitarios

```powershell
# Domain Layer
Write-Host "?? Ejecutando tests de Domain..." -ForegroundColor Cyan
dotnet test tests/Domain.UnitTests/Domain.UnitTests.csproj --verbosity normal

# Application Layer
Write-Host "?? Ejecutando tests de Application..." -ForegroundColor Cyan
dotnet test tests/Application.UnitTests/Application.UnitTests.csproj --verbosity normal
```

### 2. Generar Cobertura Completa

```powershell
# Función para generar cobertura
function Get-TestCoverage {
    param(
        [string]$ProjectPath,
 [string]$ProjectName
    )
    
    Write-Host "`n?? Generando cobertura para $ProjectName..." -ForegroundColor Green
    
    dotnet test $ProjectPath `
        /p:CollectCoverage=true `
        /p:CoverletOutputFormat=cobertura `
        /p:CoverletOutput="./TestResults/" `
  /p:Exclude="[*.UnitTests]*,[*.FunctionalTests]*" `
        --verbosity minimal
  
    if ($LASTEXITCODE -eq 0) {
  Write-Host "? Cobertura generada exitosamente para $ProjectName" -ForegroundColor Green
} else {
        Write-Host "? Error generando cobertura para $ProjectName" -ForegroundColor Red
    }
}

# Ejecutar para ambos proyectos
Get-TestCoverage -ProjectPath "tests/Domain.UnitTests/Domain.UnitTests.csproj" -ProjectName "Domain"
Get-TestCoverage -ProjectPath "tests/Application.UnitTests/Application.UnitTests.csproj" -ProjectName "Application"
```

### 3. Generar Reporte HTML

```powershell
# Verificar si reportgenerator está instalado
if (-not (Get-Command reportgenerator -ErrorAction SilentlyContinue)) {
    Write-Host "?? Instalando reportgenerator..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-reportgenerator-globaltool
}

# Generar reporte
Write-Host "`n?? Generando reporte HTML..." -ForegroundColor Cyan

reportgenerator `
    -reports:"tests/**/TestResults/coverage.cobertura.xml" `
    -targetdir:"TestResults/CoverageReport" `
    -reporttypes:"Html;Badges"

if ($LASTEXITCODE -eq 0) {
    Write-Host "? Reporte generado exitosamente" -ForegroundColor Green
    Write-Host "`n?? Abriendo reporte en el navegador..." -ForegroundColor Cyan
Start-Process "TestResults/CoverageReport/index.html"
} else {
    Write-Host "? Error generando reporte" -ForegroundColor Red
}
```

### 4. Ver Estadísticas Rápidas

```powershell
# Contar tests por proyecto
Write-Host "`n?? Estadísticas de Tests:" -ForegroundColor Cyan

$domainTests = (dotnet test tests/Domain.UnitTests --list-tests | Measure-Object).Count - 5
$appTests = (dotnet test tests/Application.UnitTests --list-tests | Measure-Object).Count - 5

Write-Host "Domain Tests: $domainTests" -ForegroundColor Green
Write-Host "Application Tests: $appTests" -ForegroundColor Green
Write-Host "Total Tests: $($domainTests + $appTests)" -ForegroundColor Green
```

## Script Completo - RunCoverage.ps1

Crea un archivo `RunCoverage.ps1` con el siguiente contenido:

```powershell
#!/usr/bin/env pwsh

# Colores para output
$ErrorActionPreference = "Stop"

function Write-Header {
  param([string]$Message)
    Write-Host "`n?????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host " $Message" -ForegroundColor Cyan
    Write-Host "?????????????????????????????????????????????????????????????????`n" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "? $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
  Write-Host "??  $Message" -ForegroundColor Cyan
}

function Write-Error {
    param([string]$Message)
 Write-Host "? $Message" -ForegroundColor Red
}

# Limpiar resultados anteriores
Write-Header "Limpiando resultados anteriores"
if (Test-Path "TestResults") {
    Remove-Item -Path "TestResults" -Recurse -Force
    Write-Success "Resultados anteriores eliminados"
}

# Verificar que existe el proyecto
if (-not (Test-Path "FinalProject.sln")) {
    Write-Error "No se encontró FinalProject.sln en el directorio actual"
    exit 1
}

# Compilar solución
Write-Header "Compilando solución"
dotnet build --configuration Release --no-incremental
if ($LASTEXITCODE -ne 0) {
    Write-Error "Error al compilar la solución"
    exit 1
}
Write-Success "Compilación exitosa"

# Ejecutar tests de Domain
Write-Header "Ejecutando tests de Domain Layer"
dotnet test tests/Domain.UnitTests/Domain.UnitTests.csproj `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput="./TestResults/" `
    /p:Exclude="[*.UnitTests]*" `
    --configuration Release `
    --no-build `
    --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Error "Algunos tests de Domain fallaron"
    exit 1
}
Write-Success "Tests de Domain completados"

# Ejecutar tests de Application
Write-Header "Ejecutando tests de Application Layer"
dotnet test tests/Application.UnitTests/Application.UnitTests.csproj `
/p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput="./TestResults/" `
    /p:Exclude="[*.UnitTests]*" `
    --configuration Release `
    --no-build `
    --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Error "Algunos tests de Application fallaron"
    exit 1
}
Write-Success "Tests de Application completados"

# Verificar si reportgenerator está instalado
Write-Header "Generando reporte de cobertura"
if (-not (Get-Command reportgenerator -ErrorAction SilentlyContinue)) {
    Write-Info "Instalando reportgenerator..."
    dotnet tool install --global dotnet-reportgenerator-globaltool
}

# Generar reporte HTML
reportgenerator `
    -reports:"tests/**/TestResults/coverage.cobertura.xml" `
    -targetdir:"TestResults/CoverageReport" `
  -reporttypes:"Html;HtmlSummary;Badges;TextSummary"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Error generando reporte de cobertura"
    exit 1
}

Write-Success "Reporte de cobertura generado"

# Mostrar resumen
Write-Header "Resumen de Cobertura"

if (Test-Path "TestResults/CoverageReport/Summary.txt") {
    Get-Content "TestResults/CoverageReport/Summary.txt"
}

# Abrir reporte en navegador
Write-Info "Abriendo reporte en el navegador..."
Start-Process "TestResults/CoverageReport/index.html"

Write-Header "¡Proceso completado exitosamente!"
Write-Success "Los tests unitarios han sido ejecutados"
Write-Success "El reporte de cobertura está disponible en: TestResults/CoverageReport/index.html"
```

### Uso del Script

```powershell
# Dar permisos de ejecución (si es necesario)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Ejecutar el script
./RunCoverage.ps1
```

## Comandos Adicionales Útiles

### Ejecutar solo un test específico
```powershell
dotnet test --filter "FullyQualifiedName~TodoItemTests"
```

### Ver lista de todos los tests
```powershell
dotnet test --list-tests
```

### Ejecutar tests con logs detallados
```powershell
dotnet test --verbosity detailed
```

### Limpiar y rebuilder todo
```powershell
dotnet clean
dotnet build
dotnet test
```

### Ver cobertura en consola (sin HTML)
```powershell
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=console
```

## Integración con CI/CD

### GitHub Actions Example

```yaml
name: Test Coverage

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
   uses: actions/setup-dotnet@v3
  with:
          dotnet-version: '9.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Test with Coverage
        run: |
 dotnet test tests/Domain.UnitTests --no-build --verbosity normal \
     /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
        
      dotnet test tests/Application.UnitTests --no-build --verbosity normal \
  /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
      
      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
       files: tests/**/coverage.opencover.xml
```

### Azure DevOps Example

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '9.0.x'

- task: DotNetCoreCLI@2
  displayName: 'Restore'
  inputs:
    command: 'restore'

- task: DotNetCoreCLI@2
  displayName: 'Build'
  inputs:
    command: 'build'
  arguments: '--configuration Release'

- task: DotNetCoreCLI@2
  displayName: 'Test with Coverage'
  inputs:
    command: 'test'
    arguments: '--configuration Release /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura'

- task: PublishCodeCoverageResults@1
  displayName: 'Publish Coverage'
  inputs:
    codeCoverageTool: 'Cobertura'
    summaryFileLocation: '**/coverage.cobertura.xml'
```

## Troubleshooting

### Error: "No tests found"
```powershell
# Limpiar y rebuilder
dotnet clean
dotnet build
dotnet test
```

### Error: "coverlet.collector not found"
```powershell
# Reinstalar paquete
dotnet add tests/Domain.UnitTests package coverlet.collector
dotnet add tests/Application.UnitTests package coverlet.collector
```

### Error: "reportgenerator not found"
```powershell
# Reinstalar herramienta
dotnet tool uninstall --global dotnet-reportgenerator-globaltool
dotnet tool install --global dotnet-reportgenerator-globaltool
```

---

**?? Tip**: Guarda estos comandos en un archivo `.ps1` para reutilizarlos fácilmente.
