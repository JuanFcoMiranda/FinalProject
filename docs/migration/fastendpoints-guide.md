# ?? GUÍA PASO A PASO: Aplicar Migraciones

## ?? ANTES DE COMENZAR

**CRÍTICO**: Debes hacer un backup completo de tu base de datos antes de continuar.

---

## ?? Opción 1: Aplicar Migraciones con Script SQL (RECOMENDADO)

Esta es la opción más confiable y te da control total sobre lo que se ejecuta.

### Paso 1: Hacer Backup

#### SQL Server
```sql
-- En SQL Server Management Studio (SSMS) o Azure Data Studio
BACKUP DATABASE [FinalProjectDb] 
TO DISK = 'C:\Backups\FinalProjectDb_PreMigration_20251021.bak'
WITH FORMAT, COMPRESSION;
```

#### Verificar que el backup se completó
```sql
RESTORE VERIFYONLY 
FROM DISK = 'C:\Backups\FinalProjectDb_PreMigration_20251021.bak';
```

### Paso 2: Ejecutar el Script de Migración

1. Abre **SQL Server Management Studio** o **Azure Data Studio**
2. Conéctate a tu base de datos
3. Abre el archivo **`ApplyMigrations.sql`**
4. Revisa el script (es seguro, tiene manejo de errores y transacciones)
5. Ejecuta el script completo (F5 o botón Execute)

### Paso 3: Verificar los Resultados

El script mostrará mensajes de progreso como:
```
=====================================================
INICIANDO MIGRACIÓN #1: RemoveTodoListDependency
=====================================================
Paso 1/3: Eliminando foreign key constraint...
  ? Foreign key eliminado correctamente
Paso 2/3: Eliminando índice IX_TodoItems_ListId...
  ? Índice eliminado correctamente
Paso 3/3: Eliminando columna ListId...
  ? Columna ListId eliminada correctamente

? MIGRACIÓN #1 COMPLETADA EXITOSAMENTE

=====================================================
INICIANDO MIGRACIÓN #2: DropTodoListsTable
=====================================================
Eliminando tabla TodoLists...
  ? Tabla TodoLists eliminada correctamente

? MIGRACIÓN #2 COMPLETADA EXITOSAMENTE

??? MIGRACIONES COMPLETADAS EXITOSAMENTE ???
```

### Paso 4: Verificar Estructura de Base de Datos

```sql
-- Verificar que ListId no existe en TodoItems
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'TodoItems';

-- Verificar que TodoLists no existe
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'TodoLists';
-- Debería retornar 0 filas

-- Verificar que los items se conservaron
SELECT COUNT(*) as TotalItems FROM TodoItems;
```

---

## ?? Opción 2: Aplicar con EF Core (Si funciona en tu entorno)

Si los comandos de EF Core funcionan en tu máquina:

### Método 1: Desde la raíz del proyecto
```powershell
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

### Método 2: Desde Visual Studio
1. Abre **Package Manager Console** (Tools > NuGet Package Manager > Package Manager Console)
2. Selecciona `src/Infrastructure` como Default Project
3. Ejecuta:
```powershell
Update-Database
```

### Método 3: Con rutas específicas
```powershell
dotnet ef database update -p src\Infrastructure\Infrastructure.csproj -s src\Web\Web.csproj
```

---

## ?? Verificación Post-Migración

### 1. Verificar Base de Datos

```sql
-- Estructura de TodoItems (sin ListId)
EXEC sp_columns 'TodoItems';

-- Verificar que los datos se conservaron
SELECT TOP 10 * FROM TodoItems;

-- Verificar que TodoLists no existe
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TodoLists';
```

### 2. Probar la Aplicación

```powershell
# Desde la raíz del proyecto
dotnet run --project src/Web
```

### 3. Probar API

#### Crear un TodoItem (sin ListId)
```bash
curl -X POST http://localhost:5000/api/TodoItems \
  -H "Content-Type: application/json" \
  -d '{"title":"Primer item sin lista"}'
```

#### Obtener lista de items
```bash
curl http://localhost:5000/api/TodoItems
```

#### Actualizar un item
```bash
curl -X PUT http://localhost:5000/api/TodoItems/1 \
  -H "Content-Type: application/json" \
-d '{"id":1,"title":"Item actualizado","done":true}'
```

### 4. Verificar Logs

Revisa los logs de la aplicación para asegurarte de que no hay errores:
- No debe haber errores de foreign key
- No debe haber referencias a `ListId`
- No debe haber errores relacionados con `TodoLists`

---

## ?? Si Algo Sale Mal: Rollback

### Opción A: Restaurar desde Backup (Recomendado)

```sql
-- Cerrar todas las conexiones a la base de datos
USE master;
GO

ALTER DATABASE [FinalProjectDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Restaurar desde backup
RESTORE DATABASE [FinalProjectDb]
FROM DISK = 'C:\Backups\FinalProjectDb_PreMigration_20251021.bak'
WITH REPLACE;
GO

ALTER DATABASE [FinalProjectDb] SET MULTI_USER;
GO
```

### Opción B: Ejecutar Script de Rollback

Si necesitas revertir las migraciones pero conservar otros cambios:

1. Abre **`RollbackMigrations.sql`**
2. Ejecuta el script completo
3. **NOTA**: Esto NO restaurará los datos de TodoLists, solo la estructura

---

## ? Checklist de Verificación

Después de aplicar las migraciones, verifica:

- [ ] ? Base de datos tiene backup reciente
- [ ] ? Script de migración ejecutado sin errores
- [ ] ? Columna `ListId` eliminada de `TodoItems`
- [ ] ? Tabla `TodoLists` eliminada
- [ ] ? Datos de `TodoItems` conservados
- [ ] ? Aplicación inicia sin errores
- [ ] ? Puedes crear un nuevo `TodoItem` sin `listId`
- [ ] ? Puedes obtener lista de `TodoItems`
- [ ] ? Puedes actualizar un `TodoItem`
- [ ] ? Puedes eliminar un `TodoItem`
- [ ] ? No hay errores en los logs
- [ ] ? Endpoints `/api/TodoLists/*` retornan 404 (esperado)

---

## ?? Qué Esperar

### ? Comportamiento Correcto
- La aplicación inicia normalmente
- Puedes crear items sin especificar `listId`
- Las operaciones CRUD funcionan correctamente
- Los endpoints `/api/TodoLists/*` ya no existen (404)

### ? Problemas Comunes y Soluciones

#### Error: "Cannot drop the table 'TodoLists', because it does not exist"
**Solución**: La tabla ya fue eliminada. Esto es normal si ejecutaste el script múltiples veces.

#### Error: "Column 'ListId' does not exist"
**Solución**: La columna ya fue eliminada. Esto es normal.

#### Error al crear TodoItem en la aplicación
**Verificar**:
1. La migración se aplicó correctamente
2. El código está actualizado (recompila: `dotnet build`)
3. No hay caché de base de datos

#### Endpoints /api/TodoLists retornan 404
**Esto es CORRECTO**: Los endpoints fueron eliminados intencionalmente.

---

## ?? Soporte

Si encuentras problemas:

1. Revisa los logs de la aplicación
2. Revisa los mensajes de error del script SQL
3. Verifica que el backup existe y es válido
4. Considera restaurar desde backup y volver a intentar

---

## ?? Próximos Pasos

Después de aplicar exitosamente las migraciones:

1. ? Probar todas las operaciones CRUD
2. ? Ejecutar suite de tests: `dotnet test`
3. ? Actualizar documentación de API (Swagger)
4. ? Notificar a consumidores de la API sobre breaking changes
5. ? Monitorear logs por 24-48 horas
6. ? Actualizar README del proyecto

---

**Fecha de Creación**: 2025-10-21
**Versión**: 1.0
**Estado**: Listo para aplicar
