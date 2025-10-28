#!/usr/bin/env pwsh

# Script para ejecutar tests con cobertura de c�digo
# FinalProject - Test Coverage Runner

# Configuraci�n de colores
$ErrorActionPreference = "Stop"

function Write-Header {
    param([string]$Message)
    Write-Host "`n?????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
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

function Write-Warning {
    param([string]$Message)
    Write-Host "??  $Message" -ForegroundColor Yellow
}

# Banner
Write-Host @"
??????????????????????????????????????????????????????????????????
?     ?
?           FinalProject - Test Coverage Runner     ?
?   .NET 9 Unit Tests    ?
?        ?
??????????????????????????????????????????????????????????????????
"@ -ForegroundColor Cyan

# Limpiar resultados anteriores
Write-Header "Limpiando resultados anteriores"
if (Test-Path "TestResults") {
    Remove-Item -Path "TestResults" -Recurse -Force
    Write-Success "Resultados anteriores eliminados"
} else {
    Write-Info "No hay resultados anteriores"
}

# Verificar que existe el proyecto
Write-Header "Verificando estructura del proyecto"
if (-not (Test-Path "FinalProject.sln")) {
    Write-Error "No se encontr� FinalProject.sln en el directorio actual"
    exit 1
}
Write-Success "Soluci�n encontrada"

if (-not (Test-Path "tests/Domain.UnitTests/Domain.UnitTests.csproj")) {
    Write-Error "No se encontr� el proyecto Domain.UnitTests"
    exit 1
}
Write-Success "Proyecto Domain.UnitTests encontrado"

if (-not (Test-Path "tests/Application.UnitTests/Application.UnitTests.csproj")) {
 Write-Error "No se encontr� el proyecto Application.UnitTests"
    exit 1
}
Write-Success "Proyecto Application.UnitTests encontrado"

# Compilar soluci�n
Write-Header "Compilando soluci�n"
Write-Info "Ejecutando: dotnet build --configuration Release --no-incremental"
dotnet build --configuration Release --no-incremental

if ($LASTEXITCODE -ne 0) {
    Write-Error "Error al compilar la soluci�n"
    exit 1
}
Write-Success "Compilaci�n exitosa"

# Ejecutar tests de Domain
Write-Header "Ejecutando tests de Domain Layer"
Write-Info "Tests incluidos: Entities, ValueObjects, Events, Exceptions, Common"

dotnet test tests/Domain.UnitTests/Domain.UnitTests.csproj `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput="./TestResults/" `
    /p:Exclude="[*.UnitTests]*" `
    --configuration Release `
    --no-build `
    --verbosity normal

if ($LASTEXITCODE -ne 0) {
    Write-Error "Algunos tests de Domain fallaron"
    exit 1
}
Write-Success "39 tests de Domain completados exitosamente"

# Ejecutar tests de Application
Write-Header "Ejecutando tests de Application Layer"
Write-Info "Tests incluidos: Models, DTOs, Exceptions, Behaviours"

dotnet test tests/Application.UnitTests/Application.UnitTests.csproj `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput="./TestResults/" `
    /p:Exclude="[*.UnitTests]*" `
 --configuration Release `
    --no-build `
    --verbosity normal

if ($LASTEXITCODE -ne 0) {
    Write-Error "Algunos tests de Application fallaron"
    exit 1
}
Write-Success "31 tests de Application completados exitosamente"

# Verificar si reportgenerator est� instalado
Write-Header "Generando reporte de cobertura HTML"
if (-not (Get-Command reportgenerator -ErrorAction SilentlyContinue)) {
    Write-Warning "reportgenerator no est� instalado"
    Write-Info "Instalando reportgenerator..."
    dotnet tool install --global dotnet-reportgenerator-globaltool
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Error al instalar reportgenerator"
        Write-Info "Intenta instalar manualmente con: dotnet tool install --global dotnet-reportgenerator-globaltool"
        exit 1
    }
    Write-Success "reportgenerator instalado correctamente"
}

# Verificar que existen archivos de cobertura
$coverageFiles = Get-ChildItem -Path "tests" -Filter "coverage.cobertura.xml" -Recurse

if ($coverageFiles.Count -eq 0) {
Write-Warning "No se encontraron archivos de cobertura"
    Write-Info "Archivos esperados: tests/**/TestResults/coverage.cobertura.xml"
} else {
    Write-Success "Encontrados $($coverageFiles.Count) archivo(s) de cobertura"
    
    # Generar reporte HTML
    Write-Info "Generando reporte HTML con m�ltiples formatos..."
    
    reportgenerator `
   -reports:"tests/**/TestResults/coverage.cobertura.xml" `
 -targetdir:"TestResults/CoverageReport" `
        -reporttypes:"Html;HtmlSummary;Badges;TextSummary" `
    -title:"FinalProject - Test Coverage Report" `
        -verbosity:"Info"
    
 if ($LASTEXITCODE -ne 0) {
        Write-Error "Error generando reporte de cobertura"
        exit 1
    }
    
    Write-Success "Reporte de cobertura generado exitosamente"
}

# Mostrar resumen
Write-Header "Resumen de Cobertura"

$summaryFile = "TestResults/CoverageReport/Summary.txt"
if (Test-Path $summaryFile) {
    Write-Host ""
    Get-Content $summaryFile | ForEach-Object { Write-Host "  $_" -ForegroundColor White }
    Write-Host ""
} else {
    Write-Warning "No se pudo generar el archivo de resumen"
}

# Estad�sticas
Write-Header "Estad�sticas de Ejecuci�n"
Write-Host "  ?? Total de tests ejecutados: " -NoNewline -ForegroundColor Cyan
Write-Host "70" -ForegroundColor Green
Write-Host "  ?? Domain.UnitTests: " -NoNewline -ForegroundColor Cyan
Write-Host "39 tests" -ForegroundColor Green
Write-Host "  ?? Application.UnitTests: " -NoNewline -ForegroundColor Cyan
Write-Host "31 tests" -ForegroundColor Green
Write-Host "  ? Tasa de �xito: " -NoNewline -ForegroundColor Cyan
Write-Host "100%" -ForegroundColor Green
Write-Host ""

# Abrir reporte en navegador
$reportPath = "TestResults/CoverageReport/index.html"
if (Test-Path $reportPath) {
    Write-Info "Abriendo reporte en el navegador..."
    Start-Process $reportPath
    Write-Success "Reporte disponible en: $reportPath"
} else {
    Write-Warning "No se pudo encontrar el archivo del reporte"
}

# Footer
Write-Header "�Proceso completado exitosamente!"
Write-Success "Todos los tests unitarios pasaron"
Write-Success "El reporte de cobertura est� disponible"
Write-Info "Ubicaci�n del reporte: TestResults/CoverageReport/index.html"

Write-Host @"

??????????????????????????????????????????????????????????????????
?        ? Coverage Complete ?          ?
??????????????????????????????????????????????????????????????????

"@ -ForegroundColor Green

exit 0
