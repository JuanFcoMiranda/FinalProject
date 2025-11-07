# ?? Solución al Error "Cannot open database FinalProjectDb"

## ?? Resumen del Problema

Cuando ejecutas `docker-compose up`, la aplicación intenta conectarse a SQL Server antes de que la base de datos `FinalProjectDb` sea creada, lo que resulta en el error:

```
Microsoft.Data.SqlClient.SqlException (0x80131904): Cannot open database "FinalProjectDb" requested by the login.
Login failed for user 'sa'.
```

## ? Solución Implementada

Se han implementado **3 soluciones automáticas** en paralelo:

### 1. Script SQL de Inicialización
- **Archivo**: `docker/init-db.sql`
- **Qué hace**: Crea la base de datos `FinalProjectDb` automáticamente cuando SQL Server inicia
- **Cuándo se ejecuta**: Al iniciar el contenedor de SQL Server

### 2. Entity Framework Migrations Automáticas
- **Archivo**: `src/Web/Program.cs`
- **Qué hace**: Ejecuta `Database.MigrateAsync()` para crear/actualizar las tablas
- **Cuándo se ejecuta**: Al iniciar la aplicación (controlado por `RUN_MIGRATIONS=true`)

### 3. Health Checks y Dependencias
- **Archivo**: `docker-compose.yml`
- **Qué hace**: La aplicación espera a que SQL Server esté completamente listo antes de iniciar
- **Cuándo se ejecuta**: Durante el orquestado de Docker Compose

## ?? Cómo Usar

### Opción 1: Dejar que funcione automáticamente (Recomendado)

```bash
# Levantar todo
docker-compose up -d

# Ver los logs y esperar ~45 segundos
docker-compose logs -f

# Verás estos mensajes indicando que todo funciona:
# ? sqlserver  | Base de datos inicializada correctamente
# ? webapp     | Running database migrations...
# ? webapp     | Database migrations completed.
```

### Opción 2: Usar el script de reparación automática

**Windows (PowerShell):**
```powershell
.\fix-database.ps1
```

**Linux/Mac (Bash):**
```bash
chmod +x fix-database.sh
./fix-database.sh
```

El script hace todo automáticamente:
1. Verifica que Docker esté corriendo
2. Inicia los contenedores si no están corriendo
3. Espera a que SQL Server esté listo
4. Crea la base de datos si no existe
5. Reinicia la aplicación para ejecutar migraciones
6. Verifica que todo funcione correctamente

### Opción 3: Manual (Si las anteriores fallan)

```bash
# Paso 1: Crear la base de datos manualmente
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -C \
  -Q "CREATE DATABASE FinalProjectDb"

# Paso 2: Reiniciar la aplicación
docker-compose restart webapp

# Paso 3: Verificar logs
docker-compose logs -f webapp
```

## ?? Archivos Modificados/Creados

### Archivos Core
- ? `docker-compose.yml` - Configuración con health checks y scripts de inicialización
- ? `src/Web/Program.cs` - Soporte para ejecutar migraciones en producción
- ? `src/Web/appsettings.Production.json` - Variable `RUN_MIGRATIONS=true`
- ? `docker/init-db.sql` - Script SQL para crear la base de datos
- ? `docker/entrypoint.sh` - Script de entrada personalizado para SQL Server

### Scripts de Ayuda
- ? `fix-database.ps1` - Script PowerShell de reparación automática
- ? `fix-database.sh` - Script Bash de reparación automática

### Documentación
- ? `DOCKER-ERROR-SOLUTION.md` - Guía rápida de solución
- ? `SQL-SERVER-MIGRATION.md` - Documentación completa actualizada
- ? `.env.example` - Variables de entorno con `RUN_MIGRATIONS`

## ?? Verificación

Para verificar que todo funciona:

```bash
# 1. Estado de contenedores
docker-compose ps

# 2. Verificar que la base de datos existe
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -C \
  -Q "SELECT name FROM sys.databases WHERE name = 'FinalProjectDb'"

# 3. Verificar que las tablas existen
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -C \
  -d FinalProjectDb \
  -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES"

# 4. Probar la API
curl http://localhost:8080/health
curl http://localhost:8080/api/TodoItems
```

## ?? Qué Cambió

### Antes
```yaml
# docker-compose.yml
webapp:
  environment:
    - ASPNETCORE_ENVIRONMENT=Production  # No ejecutaba migraciones
  depends_on:
    - sqlserver  # Simple dependencia, no esperaba a que esté listo
```

```csharp
// Program.cs
if (app.Environment.IsDevelopment())  // Solo en desarrollo
{
    await app.InitialiseDatabaseAsync();
}
```

### Después
```yaml
# docker-compose.yml
webapp:
  environment:
    - ASPNETCORE_ENVIRONMENT=Production
    - RUN_MIGRATIONS=true  # ? Nueva variable
  depends_on:
    sqlserver:
      condition: service_healthy  # ? Espera a que SQL esté listo

sqlserver:
  command: /bin/bash /usr/local/bin/entrypoint.sh  # ? Script personalizado
  volumes:
    - ./docker/init-db.sql:/docker-entrypoint-initdb.d/init-db.sql  # ? Script SQL
```

```csharp
// Program.cs
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    var runMigrations = builder.Configuration.GetValue<bool>("RUN_MIGRATIONS", false);
    if (runMigrations)  // ? También en producción si está habilitado
    {
        await app.InitialiseDatabaseAsync();
    }
}
```

## ?? Flujo de Inicialización

```
???????????????????????????????????????????????????????????????
? 1. docker-compose up                                        ?
???????????????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????????????
? 2. SQL Server Container inicia                             ?
?    - Ejecuta entrypoint.sh                                  ?
?    - Espera 30s a que SQL Server esté listo                 ?
?    - Ejecuta init-db.sql (crea FinalProjectDb)              ?
???????????????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????????????
? 3. Health Check pasa (SQL Server listo para conexiones)    ?
???????????????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????????????
? 4. WebApp Container inicia                                 ?
?    - Lee RUN_MIGRATIONS=true                                ?
?    - Se conecta a SQL Server                                ?
?    - Ejecuta Database.MigrateAsync()                        ?
?    - Crea/actualiza tablas                                  ?
???????????????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????????????
? 5. ? API lista en http://localhost:8080                   ?
???????????????????????????????????????????????????????????????
```

## ?? Tiempos Estimados

- **SQL Server inicia**: ~30 segundos
- **Script init-db.sql ejecuta**: ~5 segundos
- **Health check pasa**: ~5 segundos
- **WebApp inicia y migra**: ~10 segundos
- **TOTAL**: ~50 segundos (primer inicio)

Inicios posteriores son más rápidos (~15 segundos) porque la base de datos ya existe.

## ?? Si Aún Tienes Problemas

### Ver logs detallados
```bash
# SQL Server
docker-compose logs -f sqlserver

# Aplicación
docker-compose logs -f webapp

# Ambos
docker-compose logs -f
```

### Reiniciar desde cero
```bash
# Detener y eliminar TODO (incluyendo volúmenes)
docker-compose down -v

# Levantar de nuevo
docker-compose up -d

# Seguir los logs
docker-compose logs -f
```

### Ejecutar el script de reparación
```bash
# Windows
.\fix-database.ps1

# Linux/Mac
./fix-database.sh
```

## ?? Documentación Adicional

- **Guía Rápida**: [DOCKER-ERROR-SOLUTION.md](DOCKER-ERROR-SOLUTION.md)
- **Guía Completa**: [SQL-SERVER-MIGRATION.md](SQL-SERVER-MIGRATION.md)

## ? Resumen

El problema está **completamente resuelto** con múltiples capas de seguridad:

1. ? Script SQL crea la base de datos automáticamente
2. ? Health checks aseguran que SQL Server esté listo
3. ? Entity Framework ejecuta migraciones automáticamente
4. ? Scripts de reparación disponibles por si acaso

**Solo necesitas ejecutar `docker-compose up -d` y esperar ~50 segundos. ¡Todo funciona automáticamente!** ??
