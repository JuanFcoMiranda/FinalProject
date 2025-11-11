# ?? Guía Completa para Reducir el Tamaño de la Imagen Docker

## ?? Análisis del Tamaño Actual

**Imagen actual: 286 MB**

Desglose aproximado:
```
Base ASP.NET Alpine 9.0:     ~110 MB (38%)
ICU Libraries:               ~52 MB  (18%)
Aplicación publicada:        ~124 MB (44%)
```

---

## ?? Estrategias de Optimización

### ?? Opción 1: Trimming y Optimización de Publicación (MÁS RECOMENDADA)

**Reducción esperada: 30-40% (~80-110 MB menos)**

#### ¿Qué es Trimming?

Trimming elimina código no utilizado de las bibliotecas .NET, reduciendo significativamente el tamaño.

#### Modificación en Dockerfile

```docker
RUN dotnet publish "Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false \
    /p:PublishTrimmed=true \              # ? Habilita trimming
    /p:TrimMode=partial \                 # ? Modo parcial (más seguro)
    /p:EnableCompressionInSingleFile=true \  # ? Compresión
    /p:DebugType=None \                   # ? Sin símbolos debug
    /p:DebugSymbols=false                 # ? Sin debug symbols
```

#### Opciones de TrimMode

| Modo | Tamaño | Riesgo | Recomendación |
|------|--------|--------|---------------|
| `partial` | Medio | Bajo | ? **Recomendado** para ASP.NET Core |
| `full` | Mínimo | Alto | ?? Puede romper reflection |
| (sin trim) | Grande | Ninguno | Solo para desarrollo |

#### ?? Consideraciones

**Ventajas:**
- ? Reducción significativa de tamaño (30-40%)
- ? Compatible con la mayoría de aplicaciones ASP.NET Core
- ? FastEndpoints es compatible con trimming

**Desventajas:**
- ?? Puede causar problemas con reflection dinámica
- ?? Requiere testing exhaustivo
- ?? Entity Framework Core puede tener problemas (requiere configuración adicional)

#### Testing Necesario

```bash
# Build con trimming
docker build -t finalproject-web:trimmed .

# Probar exhaustivamente
docker run -d -p 8080:8080 \
  -e "ConnectionStrings__FinalProjectDb=..." \
  --name finalproject-trimmed \
  finalproject-web:trimmed

# Verificar logs
docker logs -f finalproject-trimmed

# Probar endpoints
curl http://localhost:8080/health
curl http://localhost:8080/api/TodoItems
```

**Tamaño esperado:** ~180-200 MB

---

### ?? Opción 2: Usar Modo Globalization Invariant (Eliminar ICU)

**Reducción esperada: ~50 MB**

Si tu aplicación **NO necesita** soporte completo de internacionalización (fechas, monedas, culturas), puedes eliminar ICU.

#### Modificación en Dockerfile

```docker
# ==================================================
# Stage 3: Final runtime image
# ==================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Solo curl, sin ICU
RUN apk add --no-cache curl

# Expose ports
EXPOSE 8080

# Copy published files
COPY --from=build /app/publish .

# Create non-root user
RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser && \
    chown -R appuser:appuser /app

USER appuser

# Environment variables - IMPORTANTE: Cambiar a invariant
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true  # ? Cambiar a true

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "FinalProject.Web.dll"]
```

#### ?? Implicaciones

**Ventajas:**
- ? Reducción de ~50 MB
- ? Imagen más ligera
- ? Inicio más rápido

**Desventajas:**
- ? Sin soporte de culturas/locales
- ? Formato de fechas solo en inglés (invariant)
- ? Sin formateo de monedas localizado
- ? Sin soporte de comparación de strings específica de cultura

**Cuándo usar:**
- APIs internas sin requisitos de i18n
- Microservicios que no manejan datos de usuario final
- APIs de backend que devuelven JSON simple

**Tamaño esperado:** ~234 MB

---

### ?? Opción 3: Comprimir con UPX (Packer de Binarios)

**Reducción esperada: 15-25%**

UPX comprime los binarios ejecutables.

```docker
# En stage build, agregar UPX
FROM restore AS build
ARG BUILD_CONFIGURATION=Release

# Install UPX
RUN apk add --no-cache upx

# ... código existente ...

RUN dotnet publish "Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Comprimir DLLs con UPX
RUN cd /app/publish && \
    find . -name "*.dll" -exec upx --best --lzma {} \; || true
```

#### ?? Consideraciones

**Ventajas:**
- ? Reducción adicional de 15-25%
- ? No afecta la funcionalidad

**Desventajas:**
- ?? Mayor tiempo de build
- ?? Mayor uso de CPU/memoria en startup (descompresión)
- ?? Puede causar problemas con algunos antivirus
- ?? No recomendado para producción crítica

**Tamaño esperado:** ~220-240 MB

---

### ?? Opción 4: Multi-Layer Caching Avanzado

Optimizar la estructura de capas para mejor aprovechamiento de caché.

```docker
# ==================================================
# Stage 1: Restore dependencies
# ==================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

# Copy only dependency-related files (más granular)
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Restore NuGet packages first (se cachea independientemente)
RUN dotnet nuget locals all --clear

COPY ["global.json", "./"]
COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

RUN dotnet restore "src/Web/Web.csproj"
```

**Tamaño:** Sin reducción de tamaño, pero builds más rápidos

---

### ?? Opción 5: Self-Contained con Trimming Agresivo

**Reducción esperada: 40-50%** pero con mayor riesgo.

```docker
RUN dotnet publish "Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    -r linux-musl-x64 \                   # ? Runtime ID para Alpine
    --self-contained true \               # ? Self-contained
    /p:PublishTrimmed=true \              # ? Trimming
    /p:TrimMode=full \                    # ?? Trimming agresivo
    /p:PublishSingleFile=true \           # ? Single file
    /p:EnableCompressionInSingleFile=true \  # ? Compresión
    /p:IncludeNativeLibrariesForSelfExtract=true
```

**?? MUY IMPORTANTE:** Requiere testing extensivo y puede romper muchas funcionalidades.

**Tamaño esperado:** ~140-160 MB

---

### ??? Opción 6: Usar imagen base más específica

**Reducción esperada: ~10-20 MB**

```docker
# Usar imagen ASP.NET Runtime Extra (sin componentes innecesarios)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine-extra AS final
```

O usar una imagen personalizada base aún más pequeña.

---

## ?? Comparación de Estrategias

| Estrategia | Reducción | Riesgo | Esfuerzo | Recomendación |
|------------|-----------|--------|----------|---------------|
| **Trimming Parcial** | ~80-110 MB | Bajo | Bajo | ? **MUY RECOMENDADA** |
| **Sin ICU (Invariant)** | ~50 MB | Medio | Bajo | ?? Solo si no necesitas i18n |
| **UPX Compression** | ~40-60 MB | Medio | Medio | ?? No recomendado para prod |
| **Trimming Full** | ~120-140 MB | Alto | Alto | ? Solo para casos específicos |
| **Self-Contained** | ~120-160 MB | Alto | Alto | ? Requiere testing extensivo |
| **Base Image Custom** | ~10-20 MB | Bajo | Alto | ?? Mantenimiento adicional |

---

## ?? Recomendación Final: Combinación Óptima

### Configuración Recomendada para Producción

Combinar **Opción 1 (Trimming Parcial)** con optimizaciones menores:

```docker
# ==================================================
# Stage 1: Restore dependencies
# ==================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

# Copy only dependency-related files
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["global.json", "./"]
COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

# Restore only Web project (will restore all referenced projects)
RUN dotnet restore "src/Web/Web.csproj"

# ==================================================
# Stage 2: Build and Publish application
# ==================================================
FROM restore AS build
ARG BUILD_CONFIGURATION=Release

# Copy only source code needed for Web project
COPY ["src/Web/", "src/Web/"]
COPY ["src/Application/", "src/Application/"]
COPY ["src/Domain/", "src/Domain/"]
COPY ["src/Infrastructure/", "src/Infrastructure/"]

# Build and publish with optimizations
WORKDIR "/src/src/Web"
RUN dotnet publish "Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false \
    /p:PublishTrimmed=true \              # ? Trimming
    /p:TrimMode=partial \                 # ? Modo parcial (seguro)
    /p:EnableCompressionInSingleFile=true \  # ? Compresión
    /p:DebugType=None \                   # ? Sin debug symbols
    /p:DebugSymbols=false \               # ? Sin debug
    /p:Optimize=true                      # ? Optimización

# ==================================================
# Stage 3: Final runtime image
# ==================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Install curl for healthcheck and ICU libraries for globalization support
RUN apk add --no-cache \
    curl \
    icu-libs \
    icu-data-full && \
    rm -rf /var/cache/apk/*              # ? Limpiar caché de apk

# Expose ports
EXPOSE 8080

# Copy published files
COPY --from=build /app/publish .

# Create non-root user
RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser && \
    chown -R appuser:appuser /app

USER appuser

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "FinalProject.Web.dll"]
```

### Tamaños Esperados

```
Sin optimizaciones:           286 MB
Con Trimming Parcial:         ~180-200 MB  (-86 MB, -30%) ?
+ Sin ICU (invariant):        ~130-150 MB  (-136 MB, -48%)
+ Trimming Full:              ~120-140 MB  (-146 MB, -51%) ??
```

---

## ?? Testing de la Imagen Optimizada

### 1. Build

```bash
# Build con optimizaciones
docker build -t finalproject-web:optimized .

# Verificar tamaño
docker images finalproject-web:optimized
```

### 2. Verificación de Funcionalidad

```bash
# Run
docker run -d -p 8080:8080 \
  -e "ConnectionStrings__FinalProjectDb=Server=localhost;..." \
  --name finalproject-opt \
  finalproject-web:optimized

# Verificar logs
docker logs -f finalproject-opt

# Probar endpoints
curl http://localhost:8080/health
curl http://localhost:8080/api/TodoItems

# Ver tamaño de capas
docker history finalproject-web:optimized
```

### 3. Tests Específicos

**Verificar que no se rompió nada:**

```bash
# 1. Health check
curl http://localhost:8080/health

# 2. Todos los endpoints
curl http://localhost:8080/api/TodoItems

# 3. Reflection (si usas)
# Verificar que Entity Framework funciona
# Verificar que MediatR funciona
# Verificar que FastEndpoints funciona

# 4. Globalización (si mantienes ICU)
# Probar formateo de fechas
# Probar diferentes culturas
```

---

## ?? Configuración Adicional en Web.csproj

Para mejorar el trimming, agrega a `src/Web/Web.csproj`:

```xml
<PropertyGroup>
    <PublishTrimmed>true</PublishTrimmed>
    <TrimMode>partial</TrimMode>
    <EnableTrimAnalyzer>true</EnableTrimAnalyzer>
    <SuppressTrimAnalysisWarnings>false</SuppressTrimAnalysisWarnings>
    <TrimmerSingleWarn>false</TrimmerSingleWarn>
</PropertyGroup>

<!-- Preservar assemblies que usan reflection -->
<ItemGroup>
    <TrimmerRootAssembly Include="FastEndpoints" />
    <TrimmerRootAssembly Include="MediatR" />
    <!-- Agregar otros si es necesario -->
</ItemGroup>
```

---

## ?? Decisión Rápida

### Para la mayoría de casos:

```dockerfile
# ? USA ESTO
/p:PublishTrimmed=true \
/p:TrimMode=partial \
/p:DebugType=None \
/p:DebugSymbols=false
```

**Tamaño esperado: ~180-200 MB** (reducción del 30%)

### Si NO necesitas i18n:

```dockerfile
# ? USA ESTO
/p:PublishTrimmed=true \
/p:TrimMode=partial \

# Y en el runtime:
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true
# NO instalar icu-libs
```

**Tamaño esperado: ~130-150 MB** (reducción del 48%)

---

## ?? Warnings y Troubleshooting

### Problema: Trimming rompe Entity Framework

**Solución:**
```xml
<ItemGroup>
    <TrimmerRootAssembly Include="Microsoft.EntityFrameworkCore" />
    <TrimmerRootAssembly Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
</ItemGroup>
```

### Problema: MediatR no encuentra handlers

**Solución:**
```xml
<ItemGroup>
    <TrimmerRootAssembly Include="MediatR" />
</ItemGroup>
```

### Problema: Reflection dinámica falla

**Solución:**
- Usar `TrimMode=partial` en lugar de `full`
- Preservar assemblies específicos con `TrimmerRootAssembly`

---

## ?? Referencias

- [.NET Trimming Options](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trimming-options)
- [Self-Contained Deployments](https://learn.microsoft.com/en-us/dotnet/core/deploying/)
- [Docker Best Practices for .NET](https://learn.microsoft.com/en-us/dotnet/core/docker/build-container)

---

## ?? Conclusión

**Recomendación final para FinalProject:**

1. ? **Implementar Trimming Parcial** (reducción del 30%)
2. ? **Mantener ICU** (necesario para i18n completo)
3. ? **Testing exhaustivo** después de aplicar cambios

**Tamaño objetivo: ~180-200 MB**

Esta configuración ofrece el mejor balance entre:
- ? Tamaño reducido
- ? Bajo riesgo
- ? Fácil implementación
- ? Mantenibilidad

---

**Autor**: GitHub Copilot  
**Fecha**: 2024  
**Proyecto**: FinalProject (.NET 9)
