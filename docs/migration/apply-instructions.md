# ?? INSTRUCCIONES CRÍTICAS: Aplicar Migración de Base de Datos

## ?? ANTES DE CONTINUAR - LEE ESTO

Esta migración **ELIMINARÁ DATOS** de tu base de datos:
- Se eliminará la columna `ListId` de la tabla `TodoItems`
- Se perderá la relación entre items y listas existentes
- Los `TodoItems` existentes seguirán en la base de datos pero sin asociación a listas

---

## ?? Pre-requisitos

1. ? Asegúrate de que el proyecto compila sin errores
2. ? **HAZ UN BACKUP COMPLETO DE TU BASE DE DATOS**
3. ? Verifica que tienes acceso a la base de datos
4. ? Lee el archivo `MIGRATION_SUMMARY.md` completamente

---

## ?? Opción 1: Aplicar Migración Directamente (Recomendado para Desarrollo)

```powershell
# Desde la raíz del proyecto
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

### Problemas Conocidos
Si recibes el error `MSB4057: The target "GetEFProjectMetadata" does not exist`, intenta:

```powershell
# Opción A: Usar rutas absolutas
cd "C:\Users\Juanfran\Developer\Visual Studio Projects\FinalProject"
dotnet ef database update --project .\src\Infrastructure\Infrastructure.csproj --startup-project .\src\Web\Web.csproj

# Opción B: Desde Visual Studio Package Manager Console
Update-Database

# Opción C: Desde el directorio src/Web
cd src\Web
dotnet ef database update --context ApplicationDbContext
```

---

## ?? Opción 2: Generar Script SQL para Revisión (Recomendado para Producción)

```powershell
# Generar script SQL
dotnet ef migrations script --project src/Infrastructure --startup-project src/Web --output remove-todolist-migration.sql

# Luego revisa el archivo remove-todolist-migration.sql
# Y ejecútalo manualmente en tu base de datos
```

---

## ?? Opción 3: Aplicar Migración Manualmente

Si los comandos de EF Core no funcionan, puedes ejecutar este SQL directamente:

```sql
-- ?? ASEGÚRATE DE TENER UN BACKUP ANTES DE EJECUTAR

-- 1. Eliminar foreign key constraint
ALTER TABLE [TodoItems] DROP CONSTRAINT [FK_TodoItems_TodoLists_ListId];

-- 2. Eliminar índice
DROP INDEX [IX_TodoItems_ListId] ON [TodoItems];

-- 3. Eliminar columna ListId
ALTER TABLE [TodoItems] DROP COLUMN [ListId];

-- Verificar cambios
SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'TodoItems';
```

---

## ? Verificación Post-Migración

Después de aplicar la migración, verifica que todo funcione:

### 1. Verificar Estructura de Base de Datos
```sql
-- Verificar que ListId ya no existe en TodoItems
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'TodoItems';

-- Verificar que el constraint fue eliminado
SELECT CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME = 'TodoItems' AND CONSTRAINT_TYPE = 'FOREIGN KEY';
```

### 2. Probar la Aplicación
```powershell
# Ejecutar la aplicación
dotnet run --project src/Web

# Probar crear un TodoItem (sin ListId)
# POST /api/TodoItems
# Body: { "title": "Test Item" }

# Debería funcionar sin errores
```

### 3. Verificar Logs
- No deberían aparecer errores relacionados con `ListId`
- Las operaciones CRUD en `TodoItems` deberían funcionar normalmente

---

## ?? Cómo Revertir la Migración (Si algo sale mal)

### Opción A: Usando EF Core
```powershell
# Listar migraciones
dotnet ef migrations list --project src/Infrastructure --startup-project src/Web

# Revertir a la migración anterior
dotnet ef database update <nombre_migracion_anterior> --project src/Infrastructure --startup-project src/Web
```

### Opción B: Restaurar desde Backup
```powershell
# Si hiciste backup, simplemente restaura la base de datos
# El método depende de tu motor de base de datos (SQL Server, PostgreSQL, etc.)
```

### Opción C: Script SQL Manual de Rollback
```sql
-- ?? Solo si necesitas revertir manualmente

-- 1. Agregar columna ListId de vuelta
ALTER TABLE [TodoItems] ADD [ListId] int NOT NULL DEFAULT 0;

-- 2. Crear índice
CREATE INDEX [IX_TodoItems_ListId] ON [TodoItems] ([ListId]);

-- 3. Agregar foreign key constraint
ALTER TABLE [TodoItems] ADD CONSTRAINT [FK_TodoItems_TodoLists_ListId] 
FOREIGN KEY ([ListId]) REFERENCES [TodoLists] ([Id]) ON DELETE CASCADE;
```

---

## ?? Siguiente Paso Después de Aplicar la Migración

Una vez que hayas aplicado exitosamente la migración:

1. ? Prueba crear, actualizar y eliminar `TodoItems`
2. ? Verifica que no hay errores de foreign key
3. ? Decide si quieres eliminar completamente la entidad `TodoList`
4. ? Actualiza los tests de la aplicación
5. ? Limpia los parámetros `ListId` no utilizados en otros comandos (ver `MIGRATION_SUMMARY.md`)

---

## ? Preguntas Frecuentes

### ¿Qué pasa con mis TodoItems existentes?
- Se mantienen en la base de datos
- Solo pierden la referencia a sus listas
- Todos los demás datos (Title, Note, Priority, etc.) se conservan

### ¿Qué pasa con mis TodoLists existentes?
- Se mantienen intactas
- Simplemente ya no tienen relación con items
- Puedes eliminarlas posteriormente si quieres

### ¿Puedo seguir usando la API de TodoLists?
- Sí, los endpoints de TodoLists siguen funcionando
- Simplemente ya no tienen relación con TodoItems
- Considera eliminarlos si ya no los necesitas

### ¿Esto afecta a mis clientes de la API?
- Sí, es un breaking change
- Los clientes ya no pueden/deben enviar `ListId` al crear items
- Actualiza la documentación de tu API
- Notifica a los consumidores

---

## ?? Si Tienes Problemas

Si encuentras errores al aplicar la migración:

1. **Verifica la cadena de conexión** en `appsettings.json`
2. **Asegúrate de tener permisos** en la base de datos
3. **Revisa los logs** de Entity Framework
4. **Intenta aplicar manualmente** el script SQL
5. **Restaura desde backup** si es necesario

---

**?? IMPORTANTE**: No apliques esta migración en producción sin antes:
- Hacer backup completo
- Probar en ambiente de desarrollo/staging
- Notificar a los stakeholders
- Tener un plan de rollback

---

**Estado**: Migración creada y lista para aplicar
**Archivo de Migración**: `src/Infrastructure/Data/Migrations/20251021121625_RemoveTodoListDependency.cs`
**Fecha**: 2025-10-21
