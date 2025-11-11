# ?? Guía Rápida Docker - Solución de Errores

## ? Inicio Rápido

```bash
# 1. Levantar todo
docker-compose up -d

# 2. Verificar el estado
docker-compose ps

# 3. Ver logs (espera a que diga "Database migrations completed")
docker-compose logs -f webapp

# 4. Probar la aplicación
curl http://localhost:8080/health
```

## ?? Solución al Error "Cannot open database"

Si ves este error:
```
Microsoft.Data.SqlClient.SqlException: Cannot open database "FinalProjectDb" requested by the login.
Login failed for user 'sa'.
```

### ? Solución Automática (Recomendada)

El sistema ya está configurado para crear la base de datos automáticamente. Solo necesitas esperar un poco más:

```bash
# Ver los logs en tiempo real
docker-compose logs -f

# Deberías ver:
# sqlserver  | Base de datos inicializada correctamente
# webapp     | Running database migrations...
# webapp     | Database migrations completed.
```

### ??? Solución Manual (Si la automática falla)

**Paso 1: Crear la base de datos manualmente**
```bash
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -C \
  -Q "CREATE DATABASE FinalProjectDb"
```

**Paso 2: Reiniciar la aplicación**
```bash
docker-compose restart webapp
```

**Paso 3: Verificar**
```bash
docker-compose logs webapp | tail -20
```

### ?? Solución Nuclear (Empezar desde cero)

Si nada funciona, reinicia todo:

```bash
# 1. Detener y eliminar todo
docker-compose down -v

# 2. Levantar de nuevo
docker-compose up -d

# 3. Seguir los logs
docker-compose logs -f
```

## ?? Comandos Útiles

### Verificar que SQL Server funciona
```bash
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -C \
  -Q "SELECT name FROM sys.databases"
```

### Verificar las tablas creadas
```bash
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -C \
  -d FinalProjectDb \
  -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES"
```

### Ver logs específicos
```bash
# Solo SQL Server
docker-compose logs -f sqlserver

# Solo la aplicación
docker-compose logs -f webapp

# Últimas 50 líneas
docker-compose logs --tail=50
```

### Acceder a los contenedores
```bash
# Consola de SQL Server
docker-compose exec sqlserver /bin/bash

# Consola de la aplicación
docker-compose exec webapp /bin/sh
```

## ?? Diagnóstico de Problemas

### El puerto 1433 está ocupado
```bash
# Windows
netstat -ano | findstr 1433

# Linux/Mac
lsof -i :1433

# Detener el proceso o cambiar el puerto en docker-compose.yml
```

### SQL Server no inicia
```bash
# Ver logs detallados
docker-compose logs sqlserver

# Verificar la contraseña (debe tener mayúsculas, minúsculas, números y símbolos)
# Si es necesario, cambia SA_PASSWORD en docker-compose.yml
```

### La aplicación no se conecta
```bash
# 1. Verificar variables de entorno
docker-compose exec webapp printenv | grep DB_

# 2. Verificar conectividad
docker-compose exec webapp ping sqlserver

# 3. Ver logs de conexión
docker-compose logs webapp | grep -i "connection\|error\|exception"
```

## ?? Cambiar Contraseñas

**IMPORTANTE**: Cambia las contraseñas por defecto antes de usar en producción.

1. Edita `docker-compose.yml`:
```yaml
environment:
  - SA_PASSWORD=TuContraseñaSegura@2025
  - DB_PASSWORD=TuContraseñaSegura@2025
```

2. O usa un archivo `.env`:
```bash
cp .env.example .env
# Edita .env con tus contraseñas
docker-compose up -d
```

## ?? Flujo de Inicialización

Cuando ejecutas `docker-compose up`, esto es lo que sucede:

1. ? **SQL Server inicia** (tarda ~30 segundos)
2. ?? **Script `init-db.sql` se ejecuta** ? Crea `FinalProjectDb`
3. ? **Health check pasa** ? SQL Server está listo
4. ?? **Aplicación inicia** ? Se conecta a SQL Server
5. ?? **Migraciones se ejecutan** ? Crea tablas y esquema
6. ?? **Todo listo** ? API disponible en http://localhost:8080

**?? Tiempo estimado**: 45-60 segundos para el primer inicio.

## ?? Siguiente Paso

Una vez que todo funcione:

```bash
# Probar el endpoint de TodoItems
curl http://localhost:8080/api/TodoItems

# Debería devolver:
# {"items":[],"pageNumber":1,"totalPages":0,"totalCount":0,"hasPreviousPage":false,"hasNextPage":false}
```

## ?? Documentación Completa

Para más detalles, consulta [SQL-SERVER-MIGRATION.md](SQL-SERVER-MIGRATION.md)
