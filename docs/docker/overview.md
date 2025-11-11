# ?? Documentación del Dockerfile

## Características del Dockerfile Optimizado

Este proyecto utiliza un **Dockerfile optimizado** con múltiples stages para producir una imagen Docker pequeña, segura y eficiente.

### ?? Características Principales

| Característica | Valor |
|---------------|-------|
| **Imagen Base** | Alpine Linux (mcr.microsoft.com/dotnet/aspnet:9.0-alpine) |
| **Tamaño Final** | ~110 MB (vs ~220 MB con Debian) |
| **Arquitectura** | Multi-stage build (4 stages) |
| **Usuario** | Non-root (appuser:1000) |
| **Health Check** | ? Incluido |
| **Caché de Docker** | ? Optimizado |

### ??? Stages del Dockerfile

#### Stage 1: Restore
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
```
- Copia solo archivos de proyecto (.csproj, .props, global.json)
- Ejecuta `dotnet restore`
- **Ventaja**: Esta capa se cachea si las dependencias no cambian

#### Stage 2: Build
```dockerfile
FROM restore AS build
```
- Copia el código fuente
- Ejecuta `dotnet build --no-restore`
- **Ventaja**: Reutiliza la capa de restore si solo cambió código

#### Stage 3: Publish
```dockerfile
FROM build AS publish
```
- Ejecuta `dotnet publish --no-build --no-restore`
- **Ventaja**: Publicación rápida sin re-compilar

#### Stage 4: Final (Runtime)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
```
- Imagen Alpine ultra-ligera
- Solo runtime de ASP.NET Core (sin SDK)
- Usuario no-root
- Health check configurado

### ?? Uso

#### Build Local
```bash
# Build la imagen
docker build -t finalproject-web:latest .

# Run el contenedor
docker run -d -p 8080:8080 --name finalproject finalproject-web:latest

# Verificar health check
curl http://localhost:8080/health

# Ver logs
docker logs finalproject

# Detener y eliminar
docker stop finalproject && docker rm finalproject
```

#### Build en GitHub Actions
El workflow `.github/workflows/netcore-cicd.yml` automáticamente:
1. Build la imagen Docker
2. La pushea a Azure Container Registry (ACR)
3. Tagea con múltiples tags (latest, branch, sha)

### ? Optimización de Caché

El Dockerfile está optimizado para aprovechar la caché de Docker:

**Primera build** (sin caché):
```bash
time docker build -t finalproject-web:v1 .
# Tiempo: ~5-8 minutos
```

**Segunda build** (con caché, sin cambios):
```bash
time docker build -t finalproject-web:v2 .
# Tiempo: ~5-10 segundos ?
```

**Build después de cambiar código** (no dependencias):
```bash
# Editar src/Web/Program.cs
time docker build -t finalproject-web:v3 .
# Tiempo: ~2-3 minutos ?
```

### ?? Seguridad

El Dockerfile implementa varias mejoras de seguridad:

1. **Usuario no-root**: La aplicación se ejecuta como `appuser` (UID 1000)
2. **Imagen Alpine**: Menor superficie de ataque
3. **Multi-stage build**: Solo los binarios necesarios en la imagen final
4. **Health check**: Permite monitoreo de salud del contenedor

### ?? Health Check

El contenedor incluye un health check que verifica la disponibilidad en `/health`:

```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1
```

**Configuración**:
- **Interval**: Cada 30 segundos
- **Timeout**: Máximo 3 segundos de espera
- **Start Period**: Espera 5 segundos antes del primer check
- **Retries**: 3 intentos fallidos antes de marcar como unhealthy

**Verificar health check**:
```bash
# Con docker
docker inspect --format='{{.State.Health.Status}}' finalproject

# Con curl
curl http://localhost:8080/health
```

### ?? Variables de Entorno

El contenedor utiliza las siguientes variables de entorno:

```dockerfile
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
```

**Override en runtime**:
```bash
docker run -e ASPNETCORE_ENVIRONMENT=Development \
           -e ASPNETCORE_URLS=http://+:5000 \
           -p 5000:5000 \
           finalproject-web:latest
```

### ?? Puertos Expuestos

- **8080**: Puerto HTTP principal

### ?? Troubleshooting

#### Problema: Error "curl: not found"
**Causa**: Alpine no incluye curl por defecto  
**Solución**: Ya incluido en el Dockerfile con `RUN apk add --no-cache curl`

#### Problema: Error de globalización
**Causa**: Alpine usa musl libc en lugar de glibc  
**Solución**: Ya configurado con `ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false`

#### Problema: Permisos denegados
**Causa**: Usuario no-root no tiene permisos  
**Solución**: Verificar que todos los archivos en `/app` pertenezcan a `appuser`

#### Problema: Build lento
**Causa**: Caché de Docker no se está utilizando  
**Solución**: Verificar que `.dockerignore` esté correctamente configurado

### ?? .dockerignore

El archivo `.dockerignore` está configurado para excluir:
- Directorios de build (`bin/`, `obj/`)
- Archivos de tests (`TestResults/`)
- Archivos de desarrollo (`.vs/`, `.vscode/`, `.idea/`)
- Archivos de Git (`.git/`, `.gitignore`)
- Documentación (`*.md`, `LICENSE`)

### ?? Mejores Prácticas Implementadas

? Multi-stage builds  
? Layer caching optimization  
? Non-root user  
? Health checks  
? Alpine Linux (imagen pequeña)  
? .dockerignore configurado  
? Build arguments para configuración  

### ?? Referencias

- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [Alpine Linux](https://alpinelinux.org/)
- [Health Check Best Practices](https://docs.docker.com/engine/reference/builder/#healthcheck)

---

**Última actualización**: 2024  
**Autor**: GitHub Copilot  
**Proyecto**: FinalProject (.NET 9)
