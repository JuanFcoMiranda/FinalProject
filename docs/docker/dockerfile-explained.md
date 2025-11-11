# ?? Explicación Detallada del Dockerfile

## ?? Ubicación y Contexto

**Ubicación correcta**: `/Dockerfile` (raíz del proyecto)  
**Contexto de build**: Raíz del proyecto (donde está la solución)

---

## ??? Arquitectura: Multi-Stage Build

El Dockerfile utiliza una arquitectura de **4 stages** para optimizar el tamaño final y la velocidad de build.

### ?? Comparación de Stages

| Stage | Base Image | Tamaño | Propósito |
|-------|-----------|--------|-----------|
| **restore** | SDK 9.0 | ~800 MB | Restaurar dependencias NuGet |
| **build** | (hereda de restore) | ~800 MB | Compilar código C# |
| **publish** | (hereda de build) | ~800 MB | Publicar aplicación |
| **final** | ASP.NET Alpine 9.0 | **~110 MB** | Ejecutar aplicación ? |

---

## ?? Análisis Detallado por Stage

### ?? Stage 1: RESTORE

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src
```

**¿Qué hace?**
- Crea un contenedor basado en la imagen del SDK de .NET 9.0
- Define `/src` como directorio de trabajo
- Le da el alias `restore` para referenciar después

**¿Por qué SDK y no Runtime?**
- El SDK incluye herramientas necesarias: `dotnet restore`, `dotnet build`, `dotnet publish`
- El Runtime solo puede ejecutar aplicaciones ya compiladas

---

```dockerfile
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["global.json", "./"]
```

**¿Qué son estos archivos?**
- `Directory.Build.props`: Configuración compartida de MSBuild (versionado, propiedades comunes)
- `Directory.Packages.props`: Gestión centralizada de versiones de paquetes NuGet
- `global.json`: Define la versión del SDK de .NET a usar

**¿Por qué copiarlos primero?**
- Docker cachea cada instrucción `COPY` como una capa
- Si estos archivos no cambian, la siguiente capa (restore) se reutiliza
- **Resultado**: Builds más rápidos si solo cambias código

---

```dockerfile
COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
```

**¿Por qué solo archivos .csproj?**
- Los `.csproj` definen las dependencias (NuGet packages)
- No necesitamos el código fuente todavía
- **Ventaja de caché**: Si solo cambias código C#, esta capa se reutiliza

**Estructura copiada:**
```
/src/
??? Directory.Build.props
??? Directory.Packages.props
??? global.json
??? src/
    ??? Web/
    ?   ??? Web.csproj
    ??? Application/
    ?   ??? Application.csproj
    ??? Domain/
    ?   ??? Domain.csproj
    ??? Infrastructure/
        ??? Infrastructure.csproj
```

---

```dockerfile
RUN dotnet restore "src/Web/Web.csproj"
```

**¿Qué hace `dotnet restore`?**
1. Lee `Web.csproj`
2. Encuentra referencias a otros proyectos (Application, Domain, Infrastructure)
3. Lee esos proyectos también
4. Descarga todos los paquetes NuGet necesarios
5. Los almacena en `/root/.nuget/packages/`

**¿Por qué solo Web.csproj?**
- `Web.csproj` referencia los otros proyectos
- `dotnet restore` automáticamente restaura las dependencias transitivas
- No necesitamos restaurar cada proyecto individualmente

**Ejemplo de referencias en Web.csproj:**
```xml
<ProjectReference Include="..\Application\Application.csproj" />
<ProjectReference Include="..\Infrastructure\Infrastructure.csproj" />
```

---

### ?? Stage 2: BUILD

```dockerfile
FROM restore AS build
ARG BUILD_CONFIGURATION=Release
```

**¿Qué significa `FROM restore`?**
- No descarga una nueva imagen
- Hereda TODO del stage `restore`:
  - Archivos copiados
  - Paquetes NuGet restaurados
  - Variables de entorno

**¿Qué es `ARG BUILD_CONFIGURATION`?**
- Define una variable de build
- Valor por defecto: `Release`
- Se puede sobrescribir: `docker build --build-arg BUILD_CONFIGURATION=Debug`

---

```dockerfile
COPY ["src/Web/", "src/Web/"]
COPY ["src/Application/", "src/Application/"]
COPY ["src/Domain/", "src/Domain/"]
COPY ["src/Infrastructure/", "src/Infrastructure/"]
```

**¿Por qué ahora sí todo el código?**
- Ya tenemos las dependencias restauradas (Stage 1)
- Ahora copiamos el código fuente (.cs, .cshtml, .json, etc.)
- Solo los 4 proyectos necesarios, **SIN proyectos de tests**

**Estructura final:**
```
/src/
??? src/
    ??? Web/
    ?   ??? Web.csproj
    ?   ??? Program.cs
    ?   ??? Endpoints/...
    ?   ??? ...
    ??? Application/
    ?   ??? Application.csproj
    ?   ??? TodoItems/...
    ?   ??? ...
    ??? Domain/
    ?   ??? Domain.csproj
    ?   ??? Entities/...
    ?   ??? ...
    ??? Infrastructure/
        ??? Infrastructure.csproj
        ??? Data/...
        ??? ...
```

---

```dockerfile
WORKDIR "/src/src/Web"
RUN dotnet build "Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore
```

**Desglose del comando:**
- `WORKDIR "/src/src/Web"`: Cambia al directorio del proyecto Web
- `dotnet build "Web.csproj"`: Compila el proyecto
- `-c $BUILD_CONFIGURATION`: Configuración (Release o Debug)
- `-o /app/build`: Output en `/app/build`
- `--no-restore`: No restaura de nuevo (ya lo hicimos en Stage 1)

**¿Qué genera `dotnet build`?**
- Archivos `.dll` compilados
- Archivos `.pdb` (símbolos de depuración)
- Archivos de configuración
- **Ubicación**: `/app/build/`

---

### ?? Stage 3: PUBLISH

```dockerfile
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-build \
    --no-restore \
    /p:UseAppHost=false
```

**¿Cuál es la diferencia entre `build` y `publish`?**

| Aspecto | dotnet build | dotnet publish |
|---------|--------------|----------------|
| **Propósito** | Compilar y verificar | Preparar para deployment |
| **Output** | Archivos intermedios | Archivos listos para ejecutar |
| **Optimización** | Ninguna | Optimización AOT, trimming |
| **Dependencias** | No incluidas | Incluye todas las dependencias |
| **Uso** | Desarrollo | Producción |

**Flags explicados:**
- `--no-build`: No compila de nuevo (reutiliza Stage 2)
- `--no-restore`: No restaura de nuevo (reutiliza Stage 1)
- `/p:UseAppHost=false`: No crea ejecutable nativo (usamos `dotnet` command)

**Contenido de `/app/publish/`:**
```
/app/publish/
??? FinalProject.Web.dll          # DLL principal
??? FinalProject.Application.dll
??? FinalProject.Domain.dll
??? FinalProject.Infrastructure.dll
??? appsettings.json
??? appsettings.Production.json
??? wwwroot/                       # Archivos estáticos
??? [todos los paquetes NuGet necesarios]
```

---

### ?? Stage 4: FINAL (Runtime)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
```

**?? Cambio crítico: De SDK a ASP.NET Runtime**

| Imagen | Tamaño | Contiene | Uso |
|--------|--------|----------|-----|
| **SDK** | ~800 MB | Compilador, herramientas, runtime | Build |
| **ASP.NET** | ~220 MB | Solo runtime de ASP.NET Core | Producción (Debian) |
| **ASP.NET Alpine** | **~110 MB** | Solo runtime de ASP.NET Core | **Producción (Alpine)** ? |

**¿Por qué Alpine?**
- Linux ultra-ligero basado en `musl libc`
- Menor superficie de ataque (seguridad)
- 50% más pequeño que Debian
- Ideal para microservicios

---

```dockerfile
RUN apk add --no-cache curl
```

**¿Qué es `apk`?**
- Gestor de paquetes de Alpine Linux (equivalente a `apt` en Debian)
- `add`: Instala paquete
- `--no-cache`: No guarda caché de índices (ahorra espacio)

**¿Por qué necesitamos curl?**
- Alpine no incluye `curl` por defecto
- Lo necesitamos para el health check
- Alternativa: Usar `wget` (viene pre-instalado en Alpine)

---

```dockerfile
EXPOSE 8080
```

**¿Qué hace `EXPOSE`?**
- **NO abre el puerto** automáticamente
- Es documentación: indica qué puerto usa la aplicación
- Para exponerlo realmente: `docker run -p 8080:8080`

**¿Por qué solo 8080?**
- Puertos < 1024 requieren privilegios de root
- Usamos usuario no-root (siguiente sección)
- HTTPS se maneja típicamente en un proxy/load balancer (nginx, Azure App Gateway)

---

```dockerfile
COPY --from=publish /app/publish .
```

**¿Qué hace `--from=publish`?**
- Copia archivos de un stage anterior (`publish`)
- **NO** copia las capas intermedias (SDK, código fuente)
- Solo los archivos publicados en `/app/publish`

**Resultado:**
```
/app/
??? FinalProject.Web.dll
??? FinalProject.Application.dll
??? FinalProject.Domain.dll
??? FinalProject.Infrastructure.dll
??? appsettings.json
??? appsettings.Production.json
??? wwwroot/
```

**Ventaja clave:**
- Imagen final NO contiene:
  - ? SDK (~600 MB)
  - ? Código fuente C#
  - ? Archivos .csproj
  - ? Archivos intermedios de build
- **Solo contiene los binarios necesarios para ejecutar**

---

```dockerfile
RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser && \
    chown -R appuser:appuser /app

USER appuser
```

**?? Seguridad: Usuario No-Root**

**¿Por qué no usar root?**
- Si la aplicación es comprometida, el atacante tiene acceso completo
- Principio de mínimo privilegio
- Requerimiento de muchas políticas de seguridad

**Desglose de comandos:**

1. `addgroup -g 1000 appuser`
   - Crea grupo `appuser` con GID 1000
   - GID 1000 es estándar para primer usuario no-root

2. `adduser -D -u 1000 -G appuser appuser`
   - `-D`: No pide password
   - `-u 1000`: UID 1000
   - `-G appuser`: Añade al grupo appuser
   - `appuser`: Nombre del usuario

3. `chown -R appuser:appuser /app`
   - Cambia ownership de `/app` y todo su contenido
   - Necesario porque los archivos fueron copiados como root

4. `USER appuser`
   - Cambia al usuario appuser para comandos siguientes
   - El `ENTRYPOINT` se ejecutará como appuser

**Verificación:**
```bash
docker exec <container> whoami
# Output: appuser
```

---

```dockerfile
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
```

**Variables de entorno explicadas:**

| Variable | Valor | Significado |
|----------|-------|-------------|
| `ASPNETCORE_URLS` | `http://+:8080` | Escucha en todas las interfaces en puerto 8080 |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Modo producción (logging mínimo, sin detalles de errores) |
| `DOTNET_RUNNING_IN_CONTAINER` | `true` | Indica a .NET que está en contenedor (optimizaciones) |
| `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT` | `false` | Habilita soporte completo de globalización en Alpine |

**¿Por qué `http://+:8080` y no `http://localhost:8080`?**
- `+` = todas las interfaces (0.0.0.0)
- `localhost` = solo loopback (no accesible desde fuera del contenedor)

**¿Por qué `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false`?**
- Alpine usa `musl libc` en lugar de `glibc`
- Por defecto, .NET usa modo invariante en Alpine (sin soporte completo de culturas)
- `false` = habilita soporte completo (necesario para formateo de fechas, monedas, etc.)

---

```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1
```

**?? Health Check**

**¿Qué es un health check?**
- Comando que Docker ejecuta periódicamente
- Determina si el contenedor está "sano" (healthy) o "enfermo" (unhealthy)
- Orquestadores (Kubernetes, Docker Swarm) lo usan para tomar decisiones

**Parámetros:**

| Parámetro | Valor | Significado |
|-----------|-------|-------------|
| `--interval` | 30s | Ejecuta cada 30 segundos |
| `--timeout` | 3s | Máximo 3 segundos de espera |
| `--start-period` | 5s | Espera 5 segundos antes del primer check |
| `--retries` | 3 | 3 fallos consecutivos = unhealthy |

**Comando:**
- `curl --fail http://localhost:8080/health`
  - Hace request GET a `/health`
  - `--fail`: Devuelve error si HTTP status >= 400
- `|| exit 1`: Si curl falla, exit code 1 (unhealthy)

**Estados posibles:**
1. `starting`: Durante el start-period
2. `healthy`: Health check exitoso
3. `unhealthy`: Después de 3 fallos consecutivos

**Verificar health:**
```bash
docker inspect --format='{{.State.Health.Status}}' <container>
# Output: healthy, unhealthy, o starting
```

**¿Dónde está el endpoint `/health`?**
```csharp
// En src/Web/Program.cs
app.UseHealthChecks("/health");
```

---

```dockerfile
ENTRYPOINT ["dotnet", "FinalProject.Web.dll"]
```

**¿Qué es `ENTRYPOINT`?**
- Comando principal que ejecuta el contenedor
- Se ejecuta cuando el contenedor inicia
- Formato exec: `["executable", "param1", "param2"]`

**¿Por qué `dotnet FinalProject.Web.dll`?**
- `dotnet`: Runtime de .NET
- `FinalProject.Web.dll`: DLL principal compilada

**¿De dónde viene `FinalProject.Web.dll`?**
- Nombre definido en `Web.csproj`:
```xml
<AssemblyName>FinalProject.Web</AssemblyName>
```

**Alternativas incorrectas:**
- ? `Web.dll` - No existe (el assembly name tiene el namespace completo)
- ? `./FinalProject.Web.dll` - Innecesario, ya estamos en `/app`

---

## ?? Flujo Completo de Ejecución

### Diagrama de Flujo

```
???????????????????????????????????????????????????????????????
? 1. docker build -t myapp .                                  ?
???????????????????????????????????????????????????????????????
                         ?
    ???????????????????????????????????????????
    ?  Stage 1: RESTORE (SDK 9.0)             ?
    ?  ????????????????????????????????????   ?
    ?  ? Copy: Directory.Build.props      ?   ?
    ?  ? Copy: Directory.Packages.props   ?   ?
    ?  ? Copy: global.json                ?   ?
    ?  ? Copy: *.csproj (4 proyectos)     ?   ?
    ?  ? Run: dotnet restore              ?   ?
    ?  ? ? Caché: Si deps no cambian     ?   ?
    ?  ????????????????????????????????????   ?
    ???????????????????????????????????????????
                         ?
    ???????????????????????????????????????????
    ?  Stage 2: BUILD (hereda restore)        ?
    ?  ????????????????????????????????????   ?
    ?  ? Copy: src/ (4 proyectos)         ?   ?
    ?  ? Run: dotnet build --no-restore   ?   ?
    ?  ? Output: /app/build/*.dll         ?   ?
    ?  ? ? Caché: Si código no cambia    ?   ?
    ?  ????????????????????????????????????   ?
    ???????????????????????????????????????????
                         ?
    ???????????????????????????????????????????
    ?  Stage 3: PUBLISH (hereda build)        ?
    ?  ????????????????????????????????????   ?
    ?  ? Run: dotnet publish              ?   ?
    ?  ? --no-build --no-restore          ?   ?
    ?  ? Output: /app/publish/*.dll       ?   ?
    ?  ????????????????????????????????????   ?
    ???????????????????????????????????????????
                         ?
    ???????????????????????????????????????????
    ?  Stage 4: FINAL (ASP.NET Alpine 9.0)    ?
    ?  ????????????????????????????????????   ?
    ?  ? Install: curl                    ?   ?
    ?  ? Copy: /app/publish ? /app        ?   ?
    ?  ? Create: appuser (non-root)       ?   ?
    ?  ? Set: ENV vars                    ?   ?
    ?  ? Set: Health check                ?   ?
    ?  ? Entry: dotnet FinalProject.*.dll ?   ?
    ?  ? ? Size: ~110 MB                 ?   ?
    ?  ????????????????????????????????????   ?
    ???????????????????????????????????????????
                         ?
    ???????????????????????????????????????????
    ? 2. docker run -p 8080:8080 myapp        ?
    ???????????????????????????????????????????
                         ?
    ???????????????????????????????????????????
    ? Container Running                        ?
    ? ????????????????????????????????????    ?
    ? ? User: appuser (UID 1000)         ?    ?
    ? ? Process: dotnet FinalProject...  ?    ?
    ? ? Port: 8080 (HTTP)                ?    ?
    ? ? Health: Every 30s ? /health      ?    ?
    ? ? Environment: Production          ?    ?
    ? ????????????????????????????????????    ?
    ???????????????????????????????????????????
```

---

## ?? Ventajas de esta Arquitectura

### 1. **Caché Eficiente**

```
Cambio en código ? Solo Stage 2, 3, 4 se reconstruyen
Cambio en deps  ? Todos los stages se reconstruyen
Sin cambios     ? Todo desde caché (~5-10 seg)
```

### 2. **Imagen Pequeña**

```
SDK (usado para build):       ~800 MB
ASP.NET Runtime Alpine:       ~110 MB  ?
ASP.NET Runtime Debian:       ~220 MB
Solo binarios necesarios
```

### 3. **Seguridad**

? Usuario non-root  
? Imagen Alpine (menor superficie ataque)  
? Multi-stage (no expone código fuente ni herramientas build)  
? Solo runtime (no compiladores ni SDKs)  

### 4. **Observabilidad**

? Health check configurado  
? Endpoints de monitoreo  
? Logs estructurados  

---

## ?? Comandos de Uso

### Build

```bash
# Build básico
docker build -t finalproject-web:latest .

# Build sin caché
docker build --no-cache -t finalproject-web:latest .

# Build con configuración Debug
docker build --build-arg BUILD_CONFIGURATION=Debug -t finalproject-web:debug .

# Ver progreso detallado
docker build --progress=plain -t finalproject-web:latest .
```

### Run

```bash
# Run básico
docker run -d -p 8080:8080 --name finalproject finalproject-web:latest

# Run con variables de entorno
docker run -d \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__DefaultConnection="..." \
  --name finalproject \
  finalproject-web:latest

# Run con límites de recursos
docker run -d \
  -p 8080:8080 \
  --memory="512m" \
  --cpus="1.0" \
  --name finalproject \
  finalproject-web:latest
```

### Verificación

```bash
# Ver logs
docker logs -f finalproject

# Health check
curl http://localhost:8080/health

# Ver health status
docker inspect --format='{{.State.Health.Status}}' finalproject

# Entrar al contenedor
docker exec -it finalproject sh

# Ver procesos
docker top finalproject

# Ver recursos
docker stats finalproject
```

---

## ? Preguntas Frecuentes

### ¿Por qué no usar `COPY . .`?

? **Malo:**
```dockerfile
COPY . .
```
- Copia TODO (incluidos bin/, obj/, tests/)
- Invalida caché fácilmente
- Imagen más grande

? **Bueno:**
```dockerfile
COPY ["src/Web/", "src/Web/"]
COPY ["src/Application/", "src/Application/"]
# ... específico
```
- Solo lo necesario
- Mejor caché
- Imagen más pequeña

---

### ¿Por qué separar restore y build?

**Sin separación:**
```dockerfile
COPY . .
RUN dotnet publish
# 8 minutos cada vez que cambias código
```

**Con separación:**
```dockerfile
# Stage 1: Restore (solo si cambian deps)
COPY *.csproj
RUN dotnet restore

# Stage 2: Build (solo si cambia código)
COPY src/
RUN dotnet build --no-restore
# Solo 2-3 minutos
```

---

### ¿Por qué Alpine y no Debian?

| Aspecto | Alpine | Debian |
|---------|--------|--------|
| **Tamaño** | ~110 MB ? | ~220 MB |
| **Seguridad** | Menor superficie ? | Mayor superficie |
| **Compatibilidad** | 99% | 100% |
| **Velocidad** | Más rápido ? | Más lento |
| **Caso de uso** | Microservicios, Cloud | Apps legacy |

**Cuándo usar Debian:**
- Necesitas paquetes específicos no disponibles en Alpine
- Problemas de compatibilidad con librerías nativas
- Estás migrando una app legacy

---

### ¿Cómo debuggear problemas de build?

```bash
# Ver logs detallados
docker build --progress=plain --no-cache -t myapp .

# Build hasta un stage específico
docker build --target=restore -t myapp:restore .
docker run -it myapp:restore sh

# Inspeccionar capas
docker history myapp:latest

# Ver tamaño de cada capa
docker history --human --format "{{.Size}}\t{{.CreatedBy}}" myapp:latest
```

---

## ?? Referencias

- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [Multi-stage Builds](https://docs.docker.com/build/building/multi-stage/)
- [Alpine Linux](https://alpinelinux.org/)

---

**Autor**: GitHub Copilot  
**Proyecto**: FinalProject (.NET 9)  
**Última actualización**: 2024
