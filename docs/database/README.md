# ?? Documentación de Base de Datos

Guías sobre configuración, migración y gestión de la base de datos SQL Server.

## ?? Documentos Disponibles

### ?? Migración
- **[sql-server-migration.md](sql-server-migration.md)** - Guía completa de migración desde LocalDB a SQL Server 2025

### ?? Solución de Problemas
- **[connection-fix.md](connection-fix.md)** - Soluciones a problemas de conexión y errores comunes

## ?? Guías Rápidas

### Configuración Inicial

```bash
# Con Docker (Recomendado)
docker-compose up -d

# Las migraciones se ejecutan automáticamente
# La base de datos se crea automáticamente
```

### Cadena de Conexión

**Desarrollo Local**:
```json
{
  "ConnectionStrings": {
    "FinalProjectDb": "Server=localhost,1433;Database=FinalProjectDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
  }
}
```

**Docker**:
```json
{
  "ConnectionStrings": {
    "FinalProjectDb": "Server=sqlserver;Database=FinalProjectDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
  }
}
```

## ??? Estructura de Base de Datos

### Tablas Principales
- `TodoItems` - Elementos de tareas pendientes

### Tablas de Identity
- `AspNetUsers` - Usuarios
- `AspNetRoles` - Roles
- `AspNetUserRoles` - Relación usuarios-roles
- `AspNetUserClaims` - Claims de usuarios
- `AspNetRoleClaims` - Claims de roles
- `AspNetUserLogins` - Logins externos
- `AspNetUserTokens` - Tokens de autenticación

### Tabla de Control
- `__EFMigrationsHistory` - Historial de migraciones de Entity Framework

## ?? Migraciones

### Ver Migraciones Disponibles
```bash
dotnet ef migrations list --project src/Infrastructure --startup-project src/Web
```

### Crear Nueva Migración
```bash
dotnet ef migrations add NombreMigracion --project src/Infrastructure --startup-project src/Web
```

### Aplicar Migraciones
```bash
# Automático (con Docker)
docker-compose up -d  # RUN_MIGRATIONS=true

# Manual
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

### Revertir Migración
```bash
dotnet ef database update NombreMigracionAnterior --project src/Infrastructure --startup-project src/Web
```

## ??? Comandos Útiles

### Verificar Estado de la Base de Datos

```bash
# Conectar con sqlcmd (Docker)
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -C

# Ver bases de datos
SELECT name FROM sys.databases;
GO

# Ver tablas
USE FinalProjectDb;
GO
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
GO

# Ver migraciones aplicadas
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory;
GO
```

### Backup y Restore

```bash
# Backup (Docker)
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C -Q "BACKUP DATABASE FinalProjectDb TO DISK = '/var/opt/mssql/backup/FinalProjectDb.bak'"

# Restore
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C -Q "RESTORE DATABASE FinalProjectDb FROM DISK = '/var/opt/mssql/backup/FinalProjectDb.bak'"
```

## ?? Problemas Comunes

### "Cannot open database" Error
? Ver [connection-fix.md](connection-fix.md)

### Migraciones no se aplican
```bash
# Verificar RUN_MIGRATIONS
docker-compose exec webapp printenv | grep RUN_MIGRATIONS

# Ver logs de migraciones
docker-compose logs webapp | grep -i migration
```

### Base de datos no se crea
```bash
# Crear manualmente
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Passw0rd -C \
  -Q "CREATE DATABASE FinalProjectDb"

# Reiniciar la aplicación
docker-compose restart webapp
```

### Conexión rechazada
```bash
# Verificar que SQL Server está corriendo
docker-compose ps

# Verificar health check
docker-compose ps | grep sqlserver

# Ver logs de SQL Server
docker-compose logs sqlserver
```

## ?? Seguridad

?? **IMPORTANTE**:

- **Desarrollo**: Usa `sa` con contraseña fuerte
- **Producción**: 
  - Crea usuarios específicos con permisos limitados
  - Usa Azure Key Vault o similar para secretos
  - No uses la cuenta `sa` en producción
  - Habilita auditoría y logging

### Crear Usuario de Aplicación (Producción)

```sql
-- Crear login
CREATE LOGIN AppUser WITH PASSWORD = 'StrongPassword123!';
GO

-- Usar la base de datos
USE FinalProjectDb;
GO

-- Crear usuario
CREATE USER AppUser FOR LOGIN AppUser;
GO

-- Dar permisos necesarios
ALTER ROLE db_datareader ADD MEMBER AppUser;
ALTER ROLE db_datawriter ADD MEMBER AppUser;
GO
```

## ?? Monitoreo

### Verificar Salud de la Base de Datos

```bash
# Health check de la aplicación
curl http://localhost:8080/health

# Ver estado de conexiones
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C -Q "SELECT * FROM sys.dm_exec_connections"
```

## ?? Enlaces Relacionados

- [Documentación de Docker](../docker/)
- [Migración Completa](sql-server-migration.md)
- [Solución de Errores de Conexión](connection-fix.md)
- [README Principal](../README.md)

## ?? Recursos Externos

- [SQL Server Documentation](https://docs.microsoft.com/sql/sql-server/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)

---

**Última actualización**: Enero 2025
