# Migración a SQL Server 2025

## Cambios Realizados

Se ha migrado la configuración de la base de datos desde LocalDB a SQL Server 2025, compatible con entornos contenerizados.

### Archivos Modificados

1. **`appsettings.json`**: Cadena de conexión base con soporte para variables de entorno
2. **`appsettings.Development.json`**: Cadena de conexión para desarrollo local con SQL Server
3. **`appsettings.Production.json`**: Cadena de conexión para producción con soporte de migraciones automáticas
4. **`docker-compose.yml`**: Orquestación de contenedores con inicialización de base de datos
5. **`.env.example`**: Ejemplo de variables de entorno
6. **`docker/init-db.sql`**: Script SQL para crear la base de datos automáticamente
7. **`docker/entrypoint.sh`**: Script de entrada personalizado para SQL Server
8. **`src/Web/Program.cs`**: Soporte para ejecutar migraciones en producción

## Configuración

### Desarrollo Local

**Opción 1: SQL Server instalado localmente**
- Usa la configuración en `appsettings.Development.json`
- Servidor: `localhost,1433`
- Usuario: `sa`
- Contraseña: `YourStrong@Passw0rd` (cámbiala por una segura)

**Opción 2: SQL Server en Docker**
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sqlserver2022 -d mcr.microsoft.com/mssql/server:2022-latest
```

### Entorno Contenerizado

1. **Copiar el archivo de variables de entorno:**
   ```bash
   cp .env.example .env
   ```

2. **Editar `.env` con tus valores (IMPORTANTE: cambiar contraseñas):**
   ```env
   SA_PASSWORD=TuContraseñaSegura@123
   DB_PASSWORD=TuContraseñaSegura@123
   ```

3. **Levantar los servicios:**
   ```bash
   docker-compose up -d
   ```

4. **Ver los logs:**
   ```bash
   # Ver todos los logs
   docker-compose logs -f
   
   # Ver solo logs de SQL Server
   docker-compose logs -f sqlserver
   
   # Ver solo logs de la aplicación
   docker-compose logs -f webapp
   ```

5. **Detener los servicios:**
   ```bash
   docker-compose down
   ```

6. **Eliminar también los volúmenes (borra la base de datos):**
   ```bash
   docker-compose down -v
   ```

### Aplicar Migraciones

Las migraciones se ejecutan automáticamente cuando se levanta el contenedor gracias a la variable `RUN_MIGRATIONS=true`.

Si necesitas ejecutarlas manualmente:

```bash
# Desarrollo local
dotnet ef database update --project src/Infrastructure --startup-project src/Web

# Verificar estado de migraciones
dotnet ef migrations list --project src/Infrastructure --startup-project src/Web
```

## Características de la Nueva Configuración

### Inicialización Automática de Base de Datos

El sistema incluye dos niveles de inicialización:

1. **Script SQL (`docker/init-db.sql`)**: Crea la base de datos `FinalProjectDb` si no existe
2. **Entity Framework Migrations**: Crea las tablas y esquema mediante `ApplicationDbContextInitialiser`

### Cadena de Conexión

- **TrustServerCertificate=True**: Necesario para SQL Server en contenedores sin certificados válidos
- **MultipleActiveResultSets=true**: Permite múltiples resultsets activos
- **ConnectRetryCount=3**: Reintentos automáticos de conexión
- **ConnectRetryInterval=5**: Intervalo de 5 segundos entre reintentos

### Docker Compose

- **Health Check**: El servicio webapp espera a que SQL Server esté listo
- **Volúmenes Persistentes**: Los datos de SQL Server persisten entre reinicios
- **Red Aislada**: Los servicios se comunican en una red privada
- **Restart Policy**: La webapp se reinicia automáticamente si falla
- **Script de Entrada Personalizado**: Ejecuta el script de inicialización antes de que la app se conecte

### Variables de Entorno

Las variables de entorno permiten configurar la conexión sin cambiar el código:

- `DB_HOST`: Servidor de base de datos
- `DB_NAME`: Nombre de la base de datos
- `DB_USER`: Usuario de la base de datos
- `DB_PASSWORD`: Contraseña del usuario
- `RUN_MIGRATIONS`: Si es `true`, ejecuta las migraciones automáticamente al iniciar

## Flujo de Inicialización

Cuando ejecutas `docker-compose up`, ocurre lo siguiente:

1. ?? **SQL Server se inicia** con el script de entrada personalizado
2. ?? **Se ejecuta `init-db.sql`** para crear la base de datos `FinalProjectDb`
3. ? **Health check pasa** cuando SQL Server acepta conexiones
4. ?? **La aplicación web se inicia** y espera a que SQL Server esté listo
5. ?? **Entity Framework ejecuta migraciones** para crear/actualizar el esquema
6. ?? **La aplicación está lista** en `http://localhost:8080`

## Notas de Seguridad

?? **IMPORTANTE**: 
- Nunca incluyas contraseñas reales en los archivos de configuración versionados
- El archivo `.env` debe estar en `.gitignore`
- Usa contraseñas fuertes en producción
- Considera usar Azure Key Vault o similar para secretos en producción

## Verificación

Para verificar que todo funciona:

```bash
# Verificar que los contenedores están corriendo
docker-compose ps

# Probar conexión a SQL Server y verificar que la base de datos existe
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C -Q "SELECT name FROM sys.databases WHERE name = 'FinalProjectDb'"

# Ver logs de la aplicación
docker-compose logs webapp

# Verificar que las migraciones se ejecutaron
docker-compose logs webapp | grep -i "migration"

# Acceder a la aplicación
curl http://localhost:8080/health

# Probar un endpoint de la API
curl http://localhost:8080/api/TodoItems
```

## Solución de Problemas

### Error: "Cannot open database FinalProjectDb requested by the login"

**Causa**: La base de datos no existe o no se creó correctamente.

**Solución**:
```bash
# 1. Verificar logs de SQL Server
docker-compose logs sqlserver

# 2. Crear la base de datos manualmente
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C -Q "CREATE DATABASE FinalProjectDb"

# 3. Reiniciar la aplicación
docker-compose restart webapp
```

### SQL Server no inicia
- Verifica que el puerto 1433 no esté en uso: `netstat -ano | findstr 1433`
- Asegúrate de que la contraseña cumple con los requisitos de SQL Server (mayúsculas, minúsculas, números y símbolos)
- Revisa los logs: `docker-compose logs sqlserver`

### La aplicación no se conecta

**Verificar la conexión**:
```bash
# 1. Verificar que SQL Server acepta conexiones
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C -Q "SELECT @@VERSION"

# 2. Verificar las variables de entorno de la aplicación
docker-compose exec webapp printenv | grep DB_

# 3. Ver logs detallados de la aplicación
docker-compose logs --tail=100 webapp
```

### Migraciones fallan

```bash
# 1. Verificar que SQL Server está corriendo
docker-compose ps

# 2. Ver logs de migraciones
docker-compose logs webapp | grep -i "migration\|database\|error"

# 3. Ejecutar migraciones manualmente desde el contenedor
docker-compose exec webapp dotnet ef database update --verbose

# 4. Verificar el estado de las migraciones
docker-compose exec webapp dotnet ef migrations list
```

### Reiniciar desde cero

Si necesitas empezar de nuevo:

```bash
# 1. Detener y eliminar todo (incluyendo volúmenes)
docker-compose down -v

# 2. Limpiar imágenes antiguas (opcional)
docker system prune -f

# 3. Levantar de nuevo
docker-compose up -d

# 4. Seguir los logs
docker-compose logs -f
```

### Acceder a SQL Server Management Studio (SSMS)

Si quieres conectarte con SSMS o Azure Data Studio:

- **Server**: `localhost,1433`
- **Authentication**: SQL Server Authentication
- **Login**: `sa`
- **Password**: `YourStrong@Passw0rd` (o la que configuraste)
- **Trust Server Certificate**: ? Yes

## Comandos Útiles

```bash
# Ver todos los contenedores
docker ps -a

# Ver logs en tiempo real
docker-compose logs -f

# Ejecutar comandos SQL
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -C -Q "SELECT * FROM sys.databases"

# Acceder a la consola de SQL Server
docker-compose exec sqlserver /bin/bash

# Acceder a la consola de la aplicación
docker-compose exec webapp /bin/sh

# Ver uso de recursos
docker stats

# Rebuild de la aplicación sin cache
docker-compose build --no-cache webapp
docker-compose up -d
