# ?? Solución Implementada - Contenedor de Base de Datos Funcional

## ? Problemas Resueltos

### 1. **Script entrypoint.sh con formato de línea incorrecto**
- **Problema**: El archivo tenía finales de línea Windows (CRLF) que causaban errores en Linux
- **Solución**: Eliminado el script personalizado. No es necesario ya que Entity Framework crea la base de datos automáticamente.

### 2. **Migraciones iniciales faltantes**
- **Problema**: Solo existían migraciones que eliminaban tablas, pero no había una migración inicial que creara las tablas
- **Solución**: Creada migración `InitialCreate` que crea todas las tablas necesarias

## ?? Archivos Modificados

### Eliminados
- `docker/entrypoint.sh` - Ya no es necesario
- `docker/init-db.sql` - Ya no es necesario

### Simplificados
- `docker-compose.yml` - Eliminadas referencias a scripts personalizados

### Mejorados
- `src/Web/Program.cs` - Mejor manejo de errores y logging en migraciones
- `src/Infrastructure/Data/ApplicationDbContextInitialiser.cs` - Más logging detallado

### Creados
- `src/Infrastructure/Data/Migrations/20251107101307_InitialCreate.cs` - Migración inicial

## ?? Cómo Funciona Ahora

```
???????????????????????????????????????????????????????????????
? 1. docker-compose up -d                                     ?
???????????????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????????????
? 2. SQL Server Container inicia (~10-12 segundos)           ?
?    - Health check verifica que esté listo para conexiones  ?
???????????????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????????????
? 3. WebApp Container inicia (después de health check)       ?
?    - Lee RUN_MIGRATIONS=true                                ?
?    - Se conecta a SQL Server                                ?
?    - Entity Framework ejecuta:                              ?
?      * CREATE DATABASE FinalProjectDb (si no existe)        ?
?      * Aplica todas las migraciones pendientes              ?
?      * Crea tablas y esquema                                ?
???????????????????????????????????????????????????????????????
                 ?
                 ?
???????????????????????????????????????????????????????????????
? 4. ? API lista en http://localhost:8080                   ?
???????????????????????????????????????????????????????????????
```

## ?? Verificación

### 1. Ver estado de contenedores
```bash
docker-compose ps
```

**Output esperado**:
```
NAME                     STATUS
finalproject-sqlserver   Up X seconds (healthy)
finalproject-webapp      Up X seconds
```

### 2. Verificar logs de migraciones
```bash
docker-compose logs webapp | Select-String -Pattern "RUN_MIGRATIONS|Database"
```

**Output esperado**:
```
RUN_MIGRATIONS is enabled. Running database initialization...
Ensuring database exists...
Database is ready and up to date.
Database initialization completed successfully.
```

### 3. Verificar base de datos
```bash
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C -d FinalProjectDb -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME"
```

**Output esperado**:
```
TABLE_NAME
__EFMigrationsHistory
AspNetRoleClaims
AspNetRoles
AspNetUserClaims
AspNetUserLogins
AspNetUserRoles
AspNetUsers
AspNetUserTokens
TodoItems

(9 rows affected)
```

### 4. Probar la API
```bash
curl http://localhost:8080/health
curl http://localhost:8080/api/TodoItems
```

**Output esperado**:
```
Healthy

{
  "items": [],
  "pageNumber": 1,
  "totalPages": 0,
  "totalCount": 0,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

## ?? Tablas Creadas

La migración `InitialCreate` crea las siguientes tablas:

### Tablas de Identity (ASP.NET Core Identity)
- `AspNetUsers` - Usuarios del sistema
- `AspNetRoles` - Roles (ej: Administrator)
- `AspNetUserRoles` - Relación usuarios-roles
- `AspNetUserClaims` - Claims de usuarios
- `AspNetRoleClaims` - Claims de roles
- `AspNetUserLogins` - Logins externos (OAuth, etc.)
- `AspNetUserTokens` - Tokens de autenticación

### Tablas de la Aplicación
- `TodoItems` - Elementos de tareas pendientes

### Tabla de Control
- `__EFMigrationsHistory` - Historial de migraciones aplicadas

## ??? Comandos Útiles

### Desarrollo

```bash
# Levantar todo
docker-compose up -d

# Ver logs en tiempo real
docker-compose logs -f

# Ver solo logs de la base de datos
docker-compose logs -f sqlserver

# Ver solo logs de la aplicación
docker-compose logs -f webapp

# Detener todo
docker-compose down

# Reiniciar desde cero (elimina volúmenes)
docker-compose down -v
docker-compose up -d
```

### Diagnóstico

```bash
# Ver estado de contenedores
docker-compose ps

# Ver logs de migraciones
docker-compose logs webapp | Select-String -Pattern "migration"

# Acceder a SQL Server
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C

# Ver migraciones aplicadas
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C -d FinalProjectDb -Q "SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory"

# Acceder a la consola de la aplicación
docker-compose exec webapp /bin/sh
```

### Reconstruir la Imagen

Si haces cambios en el código:

```bash
# Detener todo
docker-compose down

# Reconstruir la imagen
docker-compose build webapp

# O reconstruir sin cache
docker-compose build --no-cache webapp

# Levantar de nuevo
docker-compose up -d
```

## ?? Variables de Entorno

El sistema usa estas variables de entorno configuradas en `docker-compose.yml`:

```yaml
webapp:
  environment:
    - ASPNETCORE_ENVIRONMENT=Production
    - ASPNETCORE_URLS=http://+:8080
    - DB_HOST=sqlserver
    - DB_NAME=FinalProjectDb
    - DB_USER=sa
    - DB_PASSWORD=YourStrong@Passw0rd
    - RUN_MIGRATIONS=true  # ? Esta variable activa las migraciones automáticas
```

## ?? Seguridad

?? **IMPORTANTE para Producción**:

1. **Cambiar contraseñas**: Usa contraseñas fuertes y únicas
2. **Usar variables de entorno**: Crea un archivo `.env` (no versionado)
3. **Usar secretos**: Considera Azure Key Vault o Docker Secrets
4. **No exponer puertos**: En producción, solo expón el puerto 8080

### Ejemplo con archivo .env

```bash
# Crear archivo .env
cp .env.example .env

# Editar .env con contraseñas seguras
notepad .env
```

Contenido de `.env`:
```env
SA_PASSWORD=TuContraseñaSegura@2025!
DB_PASSWORD=TuContraseñaSegura@2025!
```

Modificar `docker-compose.yml` para usar variables:
```yaml
sqlserver:
  environment:
    - SA_PASSWORD=${SA_PASSWORD}

webapp:
  environment:
    - DB_PASSWORD=${DB_PASSWORD}
```

## ?? Solución de Problemas

### El contenedor de SQL Server no inicia

```bash
# Ver logs
docker-compose logs sqlserver

# Verificar puerto
netstat -ano | findstr 1433

# Detener y reiniciar
docker-compose down -v
docker-compose up -d
```

### Las migraciones no se aplican

```bash
# Verificar variable RUN_MIGRATIONS
docker-compose exec webapp printenv | Select-String RUN_MIGRATIONS

# Ver logs de migraciones
docker-compose logs webapp | Select-String -Pattern "RUN_MIGRATIONS|migration|Database"

# Aplicar migraciones manualmente
docker-compose exec webapp dotnet ef database update
```

### La API no responde

```bash
# Verificar health check
curl http://localhost:8080/health

# Ver logs de la aplicación
docker-compose logs webapp

# Verificar que las tablas existen
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C -d FinalProjectDb -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES"
```

## ?? Tiempos de Inicio

- **Primera vez** (~20-25 segundos):
  - SQL Server: 10-12 segundos
  - WebApp (con migraciones): 10-13 segundos

- **Inicios posteriores** (~15-18 segundos):
  - SQL Server: 5-7 segundos
  - WebApp (sin migraciones nuevas): 10-11 segundos

## ? Checklist de Éxito

- [ ] `docker-compose ps` muestra ambos contenedores como "Up" y SQL Server como "healthy"
- [ ] Los logs muestran "Database initialization completed successfully"
- [ ] La tabla `TodoItems` existe en la base de datos
- [ ] `curl http://localhost:8080/health` devuelve "Healthy"
- [ ] `curl http://localhost:8080/api/TodoItems` devuelve JSON válido

## ?? Siguientes Pasos

1. **Conectar con SSMS/Azure Data Studio**:
   - Server: `localhost,1433`
   - Authentication: SQL Server Authentication
   - Login: `sa`
   - Password: `YourStrong@Passw0rd`

2. **Crear datos de prueba**: Usa Postman o curl para crear TodoItems

3. **Integrar con frontend**: Configura CORS si es necesario

## ?? ¡Todo Funciona!

El contenedor de base de datos ahora funciona correctamente:
- ? SQL Server inicia sin errores
- ? La base de datos se crea automáticamente
- ? Las migraciones se aplican automáticamente
- ? La API funciona y responde correctamente
- ? Health checks pasan correctamente

**¡Puedes empezar a desarrollar!** ??
