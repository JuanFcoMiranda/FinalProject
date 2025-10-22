# ?? GU�A PASO A PASO: Aplicar Migraciones

## ?? ANTES DE COMENZAR

**CR�TICO**: Debes hacer un backup completo de tu base de datos antes de continuar.

---

## ?? Opci�n 1: Aplicar Migraciones con Script SQL (RECOMENDADO)

Esta es la opci�n m�s confiable y te da control total sobre lo que se ejecuta.

### Paso 1: Hacer Backup

#### SQL Server
```sql
-- En SQL Server Management Studio (SSMS) o Azure Data Studio
BACKUP DATABASE [FinalProjectDb] 
TO DISK = 'C:\Backups\FinalProjectDb_PreMigration_20251021.bak'
WITH FORMAT, COMPRESSION;
```

#### Verificar que el backup se complet�
```sql
RESTORE VERIFYONLY 
FROM DISK = 'C:\Backups\FinalProjectDb_PreMigration_20251021.bak';
```

### Paso 2: Ejecutar el Script de Migraci�n

1. Abre **SQL Server Management Studio** o **Azure Data Studio**
2. Con�ctate a tu base de datos
3. Abre el archivo **`ApplyMigrations.sql`**
4. Revisa el script (es seguro, tiene manejo de errores y transacciones)
5. Ejecuta el script completo (F5 o bot�n Execute)

### Paso 3: Verificar los Resultados

El script mostrar� mensajes de progreso como:
```
=====================================================
INICIANDO MIGRACI�N #1: RemoveTodoListDependency
=====================================================
Paso 1/3: Eliminando foreign key constraint...
  ? Foreign key eliminado correctamente
Paso 2/3: Eliminando �ndice IX_TodoItems_ListId...
  ? �ndice eliminado correctamente
Paso 3/3: Eliminando columna ListId...
  ? Columna ListId eliminada correctamente

? MIGRACI�N #1 COMPLETADA EXITOSAMENTE

=====================================================
INICIANDO MIGRACI�N #2: DropTodoListsTable
=====================================================
Eliminando tabla TodoLists...
  ? Tabla TodoLists eliminada correctamente

? MIGRACI�N #2 COMPLETADA EXITOSAMENTE

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
-- Deber�a retornar 0 filas

-- Verificar que los items se conservaron
SELECT COUNT(*) as TotalItems FROM TodoItems;
```

---

## ?? Opci�n 2: Aplicar con EF Core (Si funciona en tu entorno)

Si los comandos de EF Core funcionan en tu m�quina:

### M�todo 1: Desde la ra�z del proyecto
```powershell
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

### M�todo 2: Desde Visual Studio
1. Abre **Package Manager Console** (Tools > NuGet Package Manager > Package Manager Console)
2. Selecciona `src/Infrastructure` como Default Project
3. Ejecuta:
```powershell
Update-Database
```

### M�todo 3: Con rutas espec�ficas
```powershell
dotnet ef database update -p src\Infrastructure\Infrastructure.csproj -s src\Web\Web.csproj
```

---

## ?? Verificaci�n Post-Migraci�n

### 1. Verificar Base de Datos

```sql
-- Estructura de TodoItems (sin ListId)
EXEC sp_columns 'TodoItems';

-- Verificar que los datos se conservaron
SELECT TOP 10 * FROM TodoItems;

-- Verificar que TodoLists no existe
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TodoLists';
```

### 2. Probar la Aplicaci�n

```powershell
# Desde la ra�z del proyecto
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

Revisa los logs de la aplicaci�n para asegurarte de que no hay errores:
- No debe haber errores de foreign key
- No debe haber referencias a `ListId`
- No debe haber errores relacionados con `TodoLists`

---

## ?? Si Algo Sale Mal: Rollback

### Opci�n A: Restaurar desde Backup (Recomendado)

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

### Opci�n B: Ejecutar Script de Rollback

Si necesitas revertir las migraciones pero conservar otros cambios:

1. Abre **`RollbackMigrations.sql`**
2. Ejecuta el script completo
3. **NOTA**: Esto NO restaurar� los datos de TodoLists, solo la estructura

---

## ? Checklist de Verificaci�n

Despu�s de aplicar las migraciones, verifica:

- [ ] ? Base de datos tiene backup reciente
- [ ] ? Script de migraci�n ejecutado sin errores
- [ ] ? Columna `ListId` eliminada de `TodoItems`
- [ ] ? Tabla `TodoLists` eliminada
- [ ] ? Datos de `TodoItems` conservados
- [ ] ? Aplicaci�n inicia sin errores
- [ ] ? Puedes crear un nuevo `TodoItem` sin `listId`
- [ ] ? Puedes obtener lista de `TodoItems`
- [ ] ? Puedes actualizar un `TodoItem`
- [ ] ? Puedes eliminar un `TodoItem`
- [ ] ? No hay errores en los logs
- [ ] ? Endpoints `/api/TodoLists/*` retornan 404 (esperado)

---

## ?? Qu� Esperar

### ? Comportamiento Correcto
- La aplicaci�n inicia normalmente
- Puedes crear items sin especificar `listId`
- Las operaciones CRUD funcionan correctamente
- Los endpoints `/api/TodoLists/*` ya no existen (404)

### ? Problemas Comunes y Soluciones

#### Error: "Cannot drop the table 'TodoLists', because it does not exist"
**Soluci�n**: La tabla ya fue eliminada. Esto es normal si ejecutaste el script m�ltiples veces.

#### Error: "Column 'ListId' does not exist"
**Soluci�n**: La columna ya fue eliminada. Esto es normal.

#### Error al crear TodoItem en la aplicaci�n
**Verificar**:
1. La migraci�n se aplic� correctamente
2. El c�digo est� actualizado (recompila: `dotnet build`)
3. No hay cach� de base de datos

#### Endpoints /api/TodoLists retornan 404
**Esto es CORRECTO**: Los endpoints fueron eliminados intencionalmente.

---

## ?? Soporte

Si encuentras problemas:

1. Revisa los logs de la aplicaci�n
2. Revisa los mensajes de error del script SQL
3. Verifica que el backup existe y es v�lido
4. Considera restaurar desde backup y volver a intentar

---

## ?? Pr�ximos Pasos

Despu�s de aplicar exitosamente las migraciones:

1. ? Probar todas las operaciones CRUD
2. ? Ejecutar suite de tests: `dotnet test`
3. ? Actualizar documentaci�n de API (Swagger)
4. ? Notificar a consumidores de la API sobre breaking changes
5. ? Monitorear logs por 24-48 horas
6. ? Actualizar README del proyecto

---

**Fecha de Creaci�n**: 2025-10-21
**Versi�n**: 1.0
**Estado**: Listo para aplicar
