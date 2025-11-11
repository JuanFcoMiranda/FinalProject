# ?? Guía de Inicio Rápido con Docker

Esta guía te ayudará a ejecutar el proyecto FinalProject usando Docker.

## ?? Prerequisitos

- [Docker](https://www.docker.com/get-started) (v20.10 o superior)

Verificar instalación:
```bash
docker --version
```

## ?? Inicio Rápido

```bash
# 1. Clonar el repositorio
git clone https://github.com/JuanFcoMiranda/FinalProject.git
cd FinalProject

# 2. Build la imagen
docker build -t finalproject-web:latest .

# 3. Run el contenedor
docker run -d -p 8080:8080 --name finalproject finalproject-web:latest

# 4. Acceder a la aplicación
# http://localhost:8080

# 5. Verificar health check
curl http://localhost:8080/health

# 6. Ver logs
docker logs -f finalproject
```

## ?? Comandos Útiles de Docker

### Gestión Básica

```bash
# Build manual
docker build -t finalproject-web:latest .

# Build sin caché
docker build --no-cache -t finalproject-web:latest .

# Run en primer plano (ver logs directamente)
docker run -p 8080:8080 finalproject-web:latest

# Run en segundo plano
docker run -d -p 8080:8080 --name finalproject finalproject-web:latest

# Ver logs
docker logs -f finalproject

# Detener contenedor
docker stop finalproject

# Iniciar contenedor detenido
docker start finalproject

# Reiniciar contenedor
docker restart finalproject

# Eliminar contenedor
docker rm finalproject

# Forzar eliminación (si está corriendo)
docker rm -f finalproject
```

### Imágenes

```bash
# Listar imágenes
docker images

# Eliminar imagen
docker rmi finalproject-web:latest

# Eliminar imágenes no usadas
docker image prune -a

# Ver historial de capas
docker history finalproject-web:latest

# Inspeccionar imagen
docker inspect finalproject-web:latest
```

### Debugging

```bash
# Entrar al contenedor
docker exec -it finalproject sh

# Ver procesos en el contenedor
docker top finalproject

# Ver estadísticas de recursos
docker stats finalproject

# Inspeccionar contenedor
docker inspect finalproject

# Ver health check status
docker inspect --format='{{.State.Health.Status}}' finalproject

# Ver logs de los últimos 100 líneas
docker logs --tail 100 finalproject

# Ver logs desde hace 10 minutos
docker logs --since 10m finalproject
```

### Variables de Entorno

```bash
# Run con variables de entorno personalizadas
docker run -d \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ASPNETCORE_URLS=http://+:8080 \
  --name finalproject \
  finalproject-web:latest

# Run con archivo .env
docker run -d \
  -p 8080:8080 \
  --env-file .env \
  --name finalproject \
  finalproject-web:latest
```

### Límites de Recursos

```bash
# Run con límites de CPU y memoria
docker run -d \
  -p 8080:8080 \
  --cpus="1.5" \
  --memory="512m" \
  --name finalproject \
  finalproject-web:latest
```

## ?? Health Checks

Verificar el estado de salud del contenedor:

```bash
# Check health desde el host
curl http://localhost:8080/health

# Ver health status detallado
docker inspect --format='{{json .State.Health}}' finalproject | jq

# Ver estado de todos los contenedores
docker ps --format "table {{.Names}}\t{{.Status}}"
```

## ?? Troubleshooting

### Problema: Puerto ya en uso

```bash
# Ver qué está usando el puerto
lsof -i :8080  # macOS/Linux
netstat -ano | findstr :8080  # Windows

# Usar un puerto diferente
docker run -d -p 9080:8080 --name finalproject finalproject-web:latest
# Acceder en http://localhost:9080
```

### Problema: Contenedor no inicia

```bash
# Ver logs detallados
docker logs finalproject

# Ver eventos
docker events

# Inspeccionar contenedor
docker inspect finalproject

# Ver últimos contenedores (incluyendo detenidos)
docker ps -a
```

### Problema: Cambios en código no se reflejan

```bash
# Reconstruir imagen sin caché
docker build --no-cache -t finalproject-web:latest .

# Eliminar contenedor antiguo y crear uno nuevo
docker stop finalproject
docker rm finalproject
docker run -d -p 8080:8080 --name finalproject finalproject-web:latest
```

### Problema: Memoria insuficiente

```bash
# Ver uso de recursos
docker stats

# Limpiar recursos no usados
docker system prune -a --volumes

# Ver espacio usado por Docker
docker system df
```

### Problema: No puedo conectar a la aplicación

```bash
# Verificar que el contenedor está corriendo
docker ps

# Verificar los puertos expuestos
docker port finalproject

# Verificar logs
docker logs finalproject

# Probar desde dentro del contenedor
docker exec finalproject curl http://localhost:8080/health
```

## ?? Workflows de Desarrollo

### Workflow 1: Desarrollo Local con Docker

```bash
# 1. Hacer cambios en el código

# 2. Reconstruir imagen
docker build -t finalproject-web:dev .

# 3. Detener contenedor anterior
docker stop finalproject && docker rm finalproject

# 4. Run nuevo contenedor
docker run -d -p 8080:8080 --name finalproject finalproject-web:dev

# 5. Ver logs
docker logs -f finalproject
```

### Workflow 2: Testing

```bash
# Build de la imagen
docker build -t finalproject-web:test .

# Run tests dentro del contenedor
docker run --rm finalproject-web:test dotnet test

# Run tests con cobertura
docker run --rm finalproject-web:test dotnet test /p:CollectCoverage=true
```

### Workflow 3: CI/CD Local

```bash
# Simular el workflow de CI/CD localmente

# 1. Build
docker build -t finalproject-web:local .

# 2. Tag para ACR
docker tag finalproject-web:local myacr.azurecr.io/finalproject-web:latest

# 3. Login a ACR
az acr login --name myacr

# 4. Push a ACR
docker push myacr.azurecr.io/finalproject-web:latest
```

## ?? Monitoreo

### Ver uso de recursos en tiempo real

```bash
# Recursos de todos los contenedores
docker stats

# Recursos de un contenedor específico
docker stats finalproject

# Ver procesos
docker top finalproject
```

### Logs

```bash
# Ver logs en tiempo real
docker logs -f finalproject

# Ver últimas 50 líneas
docker logs --tail 50 finalproject

# Ver logs con timestamps
docker logs -t finalproject

# Ver logs desde hace 1 hora
docker logs --since 1h finalproject

# Guardar logs en archivo
docker logs finalproject > app.log
```

## ?? Limpieza

```bash
# Detener contenedor
docker stop finalproject

# Eliminar contenedor
docker rm finalproject

# Eliminar imagen
docker rmi finalproject-web:latest

# Limpieza completa del sistema Docker
docker system prune -a --volumes

# Eliminar solo imágenes no usadas
docker image prune -a

# Eliminar solo contenedores detenidos
docker container prune

# Eliminar solo volúmenes no usados
docker volume prune

# Ver espacio liberado
docker system df
```

## ?? Seguridad

### Mejores Prácticas Implementadas

? Usuario non-root  
? Imagen Alpine (menor superficie de ataque)  
? Multi-stage builds (solo binarios en imagen final)  
? Health checks  
? Variables de entorno para configuración sensible  
? .dockerignore para evitar copiar archivos innecesarios  

### Recomendaciones Adicionales

1. **No commitear secretos**: Usar variables de entorno o Docker secrets
2. **Escanear imágenes**: `docker scan finalproject-web:latest`
3. **Actualizar base images**: Mantener imágenes actualizadas
4. **Limitar recursos**: Configurar memory/cpu limits
5. **Usar tags específicos**: Evitar usar `latest` en producción

### Escaneo de Seguridad

```bash
# Escanear imagen con Docker Scout (requiere Docker Desktop)
docker scout cves finalproject-web:latest

# Ver recomendaciones
docker scout recommendations finalproject-web:latest
```

## ?? Referencias

- [Documentación Docker Detallada](DOCKER.md)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Docker CLI Reference](https://docs.docker.com/engine/reference/commandline/cli/)

## ?? Obtener Ayuda

Si encuentras problemas:

1. Revisa los logs: `docker logs -f finalproject`
2. Verifica el estado: `docker ps -a`
3. Revisa la documentación: `DOCKER.md`
4. Abre un issue en GitHub

## ?? Tips Adicionales

### Alias útiles para Bash/Zsh

```bash
# Agregar al ~/.bashrc o ~/.zshrc
alias dps='docker ps'
alias dpsa='docker ps -a'
alias di='docker images'
alias dlog='docker logs -f'
alias dstop='docker stop'
alias drm='docker rm'
alias drmi='docker rmi'
alias dclean='docker system prune -a'
```

### Script de automatización

```bash
#!/bin/bash
# deploy.sh - Script para deployar rápidamente

# Variables
IMAGE_NAME="finalproject-web"
CONTAINER_NAME="finalproject"
PORT="8080"

# Build
echo "Building image..."
docker build -t $IMAGE_NAME:latest .

# Stop y remove contenedor anterior
echo "Stopping old container..."
docker stop $CONTAINER_NAME 2>/dev/null
docker rm $CONTAINER_NAME 2>/dev/null

# Run nuevo contenedor
echo "Starting new container..."
docker run -d \
  -p $PORT:8080 \
  --name $CONTAINER_NAME \
  $IMAGE_NAME:latest

# Ver logs
echo "Container started. Viewing logs..."
docker logs -f $CONTAINER_NAME
```

---

**Última actualización**: 2024  
**Proyecto**: FinalProject (.NET 9)
