# ?? INSTRUCCIONES CR�TICAS: Aplicar Migraci�n de Base de Datos

## ?? ANTES DE CONTINUAR - LEE ESTO

Esta migraci�n **ELIMINAR� DATOS** de tu base de datos:
- Se eliminar� la columna `ListId` de la tabla `TodoItems`
- Se perder� la relaci�n entre items y listas existentes
- Los `TodoItems` existentes seguir�n en la base de datos pero sin asociaci�n a listas

---

## ?? Pre-requisitos

1. ? Aseg�rate de que el proyecto compila sin errores
2. ? **HAZ UN BACKUP COMPLETO DE TU BASE DE DATOS**
3. ? Verifica que tienes acceso a la base de datos
4. ? Lee el archivo `MIGRATION_SUMMARY.md` completamente

---

## ?? Opci�n 1: Aplicar Migraci�n Directamente (Recomendado para Desarrollo)

```powershell
# Desde la ra�z del proyecto
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

### Problemas Conocidos
Si recibes el error `MSB4057: The target "GetEFProjectMetadata" does not exist`, intenta:

```powershell
# Opci�n A: Usar rutas absolutas
cd "C:\Users\Juanfran\Developer\Visual Studio Projects\FinalProject"
dotnet ef database update --project .\src\Infrastructure\Infrastructure.csproj --startup-project .\src\Web\Web.csproj

# Opci�n B: Desde Visual Studio Package Manager Console
Update-Database

# Opci�n C: Desde el directorio src/Web
cd src\Web
dotnet ef database update --context ApplicationDbContext
```

---

## ?? Opci�n 2: Generar Script SQL para Revisi�n (Recomendado para Producci�n)

```powershell
# Generar script SQL
dotnet ef migrations script --project src/Infrastructure --startup-project src/Web --output remove-todolist-migration.sql

# Luego revisa el archivo remove-todolist-migration.sql
# Y ejec�talo manualmente en tu base de datos
```

---

## ?? Opci�n 3: Aplicar Migraci�n Manualmente

Si los comandos de EF Core no funcionan, puedes ejecutar este SQL directamente:

```sql
-- ?? ASEG�RATE DE TENER UN BACKUP ANTES DE EJECUTAR

-- 1. Eliminar foreign key constraint
ALTER TABLE [TodoItems] DROP CONSTRAINT [FK_TodoItems_TodoLists_ListId];

-- 2. Eliminar �ndice
DROP INDEX [IX_TodoItems_ListId] ON [TodoItems];

-- 3. Eliminar columna ListId
ALTER TABLE [TodoItems] DROP COLUMN [ListId];

-- Verificar cambios
SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'TodoItems';
```

---

## ? Verificaci�n Post-Migraci�n

Despu�s de aplicar la migraci�n, verifica que todo funcione:

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

### 2. Probar la Aplicaci�n
```powershell
# Ejecutar la aplicaci�n
dotnet run --project src/Web

# Probar crear un TodoItem (sin ListId)
# POST /api/TodoItems
# Body: { "title": "Test Item" }

# Deber�a funcionar sin errores
```

### 3. Verificar Logs
- No deber�an aparecer errores relacionados con `ListId`
- Las operaciones CRUD en `TodoItems` deber�an funcionar normalmente

---

## ?? C�mo Revertir la Migraci�n (Si algo sale mal)

### Opci�n A: Usando EF Core
```powershell
# Listar migraciones
dotnet ef migrations list --project src/Infrastructure --startup-project src/Web

# Revertir a la migraci�n anterior
dotnet ef database update <nombre_migracion_anterior> --project src/Infrastructure --startup-project src/Web
```

### Opci�n B: Restaurar desde Backup
```powershell
# Si hiciste backup, simplemente restaura la base de datos
# El m�todo depende de tu motor de base de datos (SQL Server, PostgreSQL, etc.)
```

### Opci�n C: Script SQL Manual de Rollback
```sql
-- ?? Solo si necesitas revertir manualmente

-- 1. Agregar columna ListId de vuelta
ALTER TABLE [TodoItems] ADD [ListId] int NOT NULL DEFAULT 0;

-- 2. Crear �ndice
CREATE INDEX [IX_TodoItems_ListId] ON [TodoItems] ([ListId]);

-- 3. Agregar foreign key constraint
ALTER TABLE [TodoItems] ADD CONSTRAINT [FK_TodoItems_TodoLists_ListId] 
FOREIGN KEY ([ListId]) REFERENCES [TodoLists] ([Id]) ON DELETE CASCADE;
```

---

## ?? Siguiente Paso Despu�s de Aplicar la Migraci�n

Una vez que hayas aplicado exitosamente la migraci�n:

1. ? Prueba crear, actualizar y eliminar `TodoItems`
2. ? Verifica que no hay errores de foreign key
3. ? Decide si quieres eliminar completamente la entidad `TodoList`
4. ? Actualiza los tests de la aplicaci�n
5. ? Limpia los par�metros `ListId` no utilizados en otros comandos (ver `MIGRATION_SUMMARY.md`)

---

## ? Preguntas Frecuentes

### �Qu� pasa con mis TodoItems existentes?
- Se mantienen en la base de datos
- Solo pierden la referencia a sus listas
- Todos los dem�s datos (Title, Note, Priority, etc.) se conservan

### �Qu� pasa con mis TodoLists existentes?
- Se mantienen intactas
- Simplemente ya no tienen relaci�n con items
- Puedes eliminarlas posteriormente si quieres

### �Puedo seguir usando la API de TodoLists?
- S�, los endpoints de TodoLists siguen funcionando
- Simplemente ya no tienen relaci�n con TodoItems
- Considera eliminarlos si ya no los necesitas

### �Esto afecta a mis clientes de la API?
- S�, es un breaking change
- Los clientes ya no pueden/deben enviar `ListId` al crear items
- Actualiza la documentaci�n de tu API
- Notifica a los consumidores

---

## ?? Si Tienes Problemas

Si encuentras errores al aplicar la migraci�n:

1. **Verifica la cadena de conexi�n** en `appsettings.json`
2. **Aseg�rate de tener permisos** en la base de datos
3. **Revisa los logs** de Entity Framework
4. **Intenta aplicar manualmente** el script SQL
5. **Restaura desde backup** si es necesario

---

**?? IMPORTANTE**: No apliques esta migraci�n en producci�n sin antes:
- Hacer backup completo
- Probar en ambiente de desarrollo/staging
- Notificar a los stakeholders
- Tener un plan de rollback

---

**Estado**: Migraci�n creada y lista para aplicar
**Archivo de Migraci�n**: `src/Infrastructure/Data/Migrations/20251021121625_RemoveTodoListDependency.cs`
**Fecha**: 2025-10-21
