# ? Checklist: Eliminaci�n COMPLETA de TodoList

## ?? Fase 1: Modificaci�n de C�digo (COMPLETADA ?)

- [x] Modificar `TodoItem.cs` - Eliminar `ListId` y `List`
- [x] Modificar `CreateTodoItem.cs` - Eliminar `ListId` del comando
- [x] Modificar `CreateTodoItemCommandValidator.cs` - Eliminar validaci�n
- [x] Modificar `UpdateTodoItemDetail.cs` - Eliminar asignaci�n de `ListId`
- [x] Modificar `GetTodoItemsWithPagination.cs` - Eliminar filtro
- [x] Crear migraciones de base de datos (2 migraciones creadas)
- [x] Verificar compilaci�n sin errores
- [x] Crear documentaci�n de cambios

## ?? Fase 4: Limpieza Completa de TodoList (COMPLETADA ?)

- [x] Decidir: Eliminar completamente TodoList ?
- [x] Eliminar `src/Domain/Entities/TodoList.cs`
- [x] Eliminar `src/Application/TodoLists/` (carpeta completa)
- [x] Eliminar `src/Web/Endpoints/TodoLists/` (carpeta completa)
- [x] Eliminar `TodoListConfiguration.cs`
- [x] Modificar `ApplicationDbContext.cs` - Sin DbSet<TodoList>
- [x] Modificar `IApplicationDbContext.cs` - Sin DbSet<TodoList>
- [x] Crear migraci�n #2 para eliminar tabla `TodoLists`
- [x] Eliminar `ListId` de `UpdateTodoItemDetailCommand`
- [x] Eliminar `ListId` de `GetTodoItemsWithPaginationQuery`
- [x] Recompilar y verificar ? Sin errores

---

## ???? Fase 2: Migraci�n de Base de Datos (EN CURSO - LISTO PARA APLICAR)

### ?? Scripts SQL Creados

He creado 3 scripts SQL para ti:

1. **`ApplyMigrations.sql`** ? - Script principal para aplicar las migraciones
   - Incluye verificaci�n pre y post-migraci�n
   - Manejo de transacciones
   - Mensajes de progreso detallados
   - Rollback autom�tico en caso de error

2. **`RollbackMigrations.sql`** - Script para revertir cambios si es necesario
   - Recrea la estructura (NO los datos)
   - �til si necesitas volver atr�s

3. **`MIGRATION_GUIDE.md`** - Gu�a completa paso a paso

### ? Pasos para Aplicar las Migraciones

#### **PASO 1: Hacer Backup (OBLIGATORIO)** ??
```sql
-- En SQL Server Management Studio o Azure Data Studio
BACKUP DATABASE [FinalProjectDb] 
TO DISK = 'C:\Backups\FinalProjectDb_PreMigration_20251021.bak'
WITH FORMAT, COMPRESSION;

-- Verificar backup
RESTORE VERIFYONLY 
FROM DISK = 'C:\Backups\FinalProjectDb_PreMigration_20251021.bak';
```

#### **PASO 2: Ejecutar Script de Migraci�n**
1. Abre SQL Server Management Studio o Azure Data Studio
2. Con�ctate a tu base de datos `FinalProjectDb`
3. Abre el archivo **`ApplyMigrations.sql`**
4. Revisa el script (tiene protecciones de seguridad)
5. Ejecuta el script completo (F5 o bot�n Execute)
6. Revisa los mensajes de salida - deben mostrar ? en todos los pasos

#### **PASO 3: Verificar en Base de Datos**
```sql
-- Verificar que ListId no existe
SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TodoItems';

-- Verificar que TodoLists no existe
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TodoLists';

-- Verificar que los items se conservaron
SELECT COUNT(*) as TotalItems FROM TodoItems;
```

#### **PASO 4: Probar la Aplicaci�n**
```powershell
# Ejecutar aplicaci�n
dotnet run --project src/Web

# En otro terminal, probar crear item
curl -X POST http://localhost:5000/api/TodoItems \
  -H "Content-Type: application/json" \
  -d '{"title":"Test Item Sin Lista"}'
```

### Checklist de Esta Fase:

- [ ] **CR�TICO**: Hacer backup completo de la base de datos
- [ ] Abrir SQL Server Management Studio / Azure Data Studio
- [ ] Ejecutar `ApplyMigrations.sql`
- [ ] Verificar mensajes de �xito en todos los pasos
- [ ] Verificar estructura de base de datos (sin ListId, sin TodoLists)
- [ ] Probar aplicaci�n: `dotnet run --project src/Web`
- [ ] Probar crear TodoItem sin ListId
- [ ] Probar actualizar TodoItem
- [ ] Probar obtener lista de TodoItems
- [ ] Probar eliminar TodoItem
- [ ] Verificar logs de aplicaci�n (sin errores)

### ?? Comandos de Verificaci�n
```sql
-- Ver estructura de TodoItems (sin ListId)
EXEC sp_columns 'TodoItems';

-- Contar items (deben estar todos)
SELECT COUNT(*) FROM TodoItems;

-- Verificar que TodoLists no existe (debe dar error)
SELECT COUNT(*) FROM TodoLists;  -- Error esperado: "Invalid object name 'TodoLists'"
```

---

## ?? Fase 3: Pruebas (PENDIENTE - Despu�s de Fase 2)

### Pruebas Manuales
- [ ] Ejecutar aplicaci�n: `dotnet run --project src/Web`
- [ ] Crear un TodoItem sin ListId
  ```json
  POST /api/TodoItems
  { "title": "Test Item" }
  ```
- [ ] Actualizar un TodoItem
- [ ] Obtener lista de TodoItems
- [ ] Eliminar un TodoItem
- [ ] Verificar que `/api/TodoLists` retorna 404 (esperado)
- [ ] Verificar que no hay errores en logs

### Pruebas Automatizadas
- [ ] Ejecutar suite de tests: `dotnet test`
- [ ] Verificar que tests de TodoItems pasan
- [ ] Arreglar tests con error de NUnit (si aplica)

---

## ?? Fase 5: Documentaci�n y Comunicaci�n (PENDIENTE - Despu�s de Fase 3)

- [ ] Actualizar README.md del proyecto
- [ ] Actualizar documentaci�n de API (Swagger/OpenAPI)
- [ ] Documentar breaking changes
- [ ] Preparar release notes
- [ ] Notificar a consumidores de la API sobre cambios:
  - `/api/TodoLists/*` ya no existe
  - `POST /api/TodoItems` ya no requiere `listId`
- [ ] Actualizar gu�as de usuario si existen

---

## ?? Fase 6: Despliegue (PENDIENTE - Cuando est� listo para producci�n)

### Desarrollo/Staging
- [ ] Desplegar en ambiente de desarrollo
- [ ] Probar exhaustivamente
- [ ] Validar con equipo de QA
- [ ] Obtener aprobaci�n

### Producci�n (cuando est� listo)
- [ ] **CR�TICO**: Backup de producci�n
- [ ] Planificar ventana de mantenimiento
- [ ] Notificar a usuarios
- [ ] Desplegar c�digo
- [ ] Ejecutar migraciones con `ApplyMigrations.sql`
- [ ] Verificar funcionalidad
- [ ] Monitorear logs y errores por 24-48 horas
- [ ] Tener plan de rollback listo (`RollbackMigrations.sql`)

---

## ? Verificaci�n Post-Implementaci�n

### Verificaciones T�cnicas
- [ ] Aplicaci�n inicia sin errores
- [ ] No hay errores de foreign key en logs
- [ ] Operaciones CRUD funcionan correctamente
- [ ] Performance es aceptable
- [ ] No hay memory leaks
- [ ] Logs no muestran advertencias inesperadas
- [ ] Endpoints `/api/TodoLists/*` retornan 404 (correcto)

### Verificaciones de Negocio
- [ ] Usuarios pueden crear items sin problemas
- [ ] Funcionalidad esperada est� operativa
- [ ] No hay quejas de usuarios
- [ ] M�tricas de uso son normales

---

## ?? Plan de Rollback (Si algo sale mal)

### Opci�n A: Restaurar desde Backup (Recomendado)
```sql
USE master;
ALTER DATABASE [FinalProjectDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [FinalProjectDb]
FROM DISK = 'C:\Backups\FinalProjectDb_PreMigration_20251021.bak'
WITH REPLACE;
ALTER DATABASE [FinalProjectDb] SET MULTI_USER;
```

### Opci�n B: Ejecutar Script de Rollback
- [ ] Abrir `RollbackMigrations.sql`
- [ ] Ejecutar script completo
- [ ] ?? NOTA: NO restaura datos, solo estructura

### Comunicaci�n de Rollback
- [ ] Notificar a stakeholders
- [ ] Documentar raz�n del rollback
- [ ] Planificar nueva fecha de implementaci�n

---

## ?? M�tricas de �xito

- [ ] ? 0 errores de foreign key constraint
- [ ] ? Tiempo de respuesta de API igual o mejor
- [ ] ? 0 errores cr�ticos en producci�n
- [ ] ? Satisfacci�n de usuarios mantenida
- [ ] ? Tests pasan al 100%

---

## ?? Recursos y Referencias

### **Documentaci�n Creada**:
- `README_CHANGES.md` - Resumen ejecutivo completo
- `MIGRATION_SUMMARY.md` - An�lisis detallado de cambios
- `APPLY_MIGRATION_INSTRUCTIONS.md` - Instrucciones originales
- `CHECKLIST.md` - Este archivo (actualizado)
- **`ApplyMigrations.sql`** ? - Script SQL para aplicar migraciones
- **`RollbackMigrations.sql`** - Script SQL para rollback
- **`MIGRATION_GUIDE.md`** - Gu�a completa paso a paso

### **Archivos Modificados**:
- Ver lista completa en `README_CHANGES.md`

### **Migraciones**:
- `src/Infrastructure/Data/Migrations/20251021121625_RemoveTodoListDependency.cs`
- `src/Infrastructure/Data/Migrations/20251021122000_DropTodoListsTable.cs`

---

## ?? Estado Actual

**Fase Actual**: ?? Fase 2 - Migraci�n de Base de Datos (LISTO PARA APLICAR)

**Completado**: 
- ? Fase 1: Modificaci�n de C�digo (100%)
- ? Fase 4: Limpieza Completa de TodoList (100%)
- ? Scripts SQL creados y listos

**Pendiente**:
- ?? **Fase 2: Aplicar migraciones a base de datos** ? SIGUIENTE PASO
- ?? Fase 3: Pruebas (despu�s de migraci�n)
- ?? Fase 5: Documentaci�n y comunicaci�n
- ?? Fase 6: Despliegue a producci�n

**Pr�xima Acci�n INMEDIATA**: 
1. **Hacer backup de base de datos** ?? CR�TICO
2. **Ejecutar `ApplyMigrations.sql`** en SQL Server Management Studio
3. **Verificar que migraciones se aplicaron correctamente**
4. **Probar aplicaci�n**

---

## ?? Estimaci�n de Tiempo

| Fase | Tiempo Estimado | Estado |
|------|-----------------|--------|
| Fase 1 (C�digo) | ? Completada | 100% |
| Fase 4 (Limpieza) | ? Completada | 100% |
| **Fase 2 (Migraci�n)** | **15-30 minutos** | **? AHORA** |
| Fase 3 (Pruebas) | 1-2 horas | Pendiente |
| Fase 5 (Documentaci�n) | 30-45 minutos | Pendiente |
| Fase 6 (Despliegue) | Variable | Pendiente |

---

**�ltima Actualizaci�n**: 2025-10-21
**Estado del Proyecto**: ? C�digo completo | ? Scripts SQL listos | ?? Migraci�n pendiente de aplicar
**Responsable**: Copilot + Usuario
**Archivos Clave**: `ApplyMigrations.sql`, `MIGRATION_GUIDE.md`
