# ? Checklist: Eliminación COMPLETA de TodoList

## ?? Fase 1: Modificación de Código (COMPLETADA ?)

- [x] Modificar `TodoItem.cs` - Eliminar `ListId` y `List`
- [x] Modificar `CreateTodoItem.cs` - Eliminar `ListId` del comando
- [x] Modificar `CreateTodoItemCommandValidator.cs` - Eliminar validación
- [x] Modificar `UpdateTodoItemDetail.cs` - Eliminar asignación de `ListId`
- [x] Modificar `GetTodoItemsWithPagination.cs` - Eliminar filtro
- [x] Crear migraciones de base de datos (2 migraciones creadas)
- [x] Verificar compilación sin errores
- [x] Crear documentación de cambios

## ?? Fase 4: Limpieza Completa de TodoList (COMPLETADA ?)

- [x] Decidir: Eliminar completamente TodoList ?
- [x] Eliminar `src/Domain/Entities/TodoList.cs`
- [x] Eliminar `src/Application/TodoLists/` (carpeta completa)
- [x] Eliminar `src/Web/Endpoints/TodoLists/` (carpeta completa)
- [x] Eliminar `TodoListConfiguration.cs`
- [x] Modificar `ApplicationDbContext.cs` - Sin DbSet<TodoList>
- [x] Modificar `IApplicationDbContext.cs` - Sin DbSet<TodoList>
- [x] Crear migración #2 para eliminar tabla `TodoLists`
- [x] Eliminar `ListId` de `UpdateTodoItemDetailCommand`
- [x] Eliminar `ListId` de `GetTodoItemsWithPaginationQuery`
- [x] Recompilar y verificar ? Sin errores

---

## ???? Fase 2: Migración de Base de Datos (EN CURSO - LISTO PARA APLICAR)

### ?? Scripts SQL Creados

He creado 3 scripts SQL para ti:

1. **`ApplyMigrations.sql`** ? - Script principal para aplicar las migraciones
   - Incluye verificación pre y post-migración
   - Manejo de transacciones
   - Mensajes de progreso detallados
   - Rollback automático en caso de error

2. **`RollbackMigrations.sql`** - Script para revertir cambios si es necesario
   - Recrea la estructura (NO los datos)
   - Útil si necesitas volver atrás

3. **`MIGRATION_GUIDE.md`** - Guía completa paso a paso

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

#### **PASO 2: Ejecutar Script de Migración**
1. Abre SQL Server Management Studio o Azure Data Studio
2. Conéctate a tu base de datos `FinalProjectDb`
3. Abre el archivo **`ApplyMigrations.sql`**
4. Revisa el script (tiene protecciones de seguridad)
5. Ejecuta el script completo (F5 o botón Execute)
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

#### **PASO 4: Probar la Aplicación**
```powershell
# Ejecutar aplicación
dotnet run --project src/Web

# En otro terminal, probar crear item
curl -X POST http://localhost:5000/api/TodoItems \
  -H "Content-Type: application/json" \
  -d '{"title":"Test Item Sin Lista"}'
```

### Checklist de Esta Fase:

- [ ] **CRÍTICO**: Hacer backup completo de la base de datos
- [ ] Abrir SQL Server Management Studio / Azure Data Studio
- [ ] Ejecutar `ApplyMigrations.sql`
- [ ] Verificar mensajes de éxito en todos los pasos
- [ ] Verificar estructura de base de datos (sin ListId, sin TodoLists)
- [ ] Probar aplicación: `dotnet run --project src/Web`
- [ ] Probar crear TodoItem sin ListId
- [ ] Probar actualizar TodoItem
- [ ] Probar obtener lista de TodoItems
- [ ] Probar eliminar TodoItem
- [ ] Verificar logs de aplicación (sin errores)

### ?? Comandos de Verificación
```sql
-- Ver estructura de TodoItems (sin ListId)
EXEC sp_columns 'TodoItems';

-- Contar items (deben estar todos)
SELECT COUNT(*) FROM TodoItems;

-- Verificar que TodoLists no existe (debe dar error)
SELECT COUNT(*) FROM TodoLists;  -- Error esperado: "Invalid object name 'TodoLists'"
```

---

## ?? Fase 3: Pruebas (PENDIENTE - Después de Fase 2)

### Pruebas Manuales
- [ ] Ejecutar aplicación: `dotnet run --project src/Web`
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

## ?? Fase 5: Documentación y Comunicación (PENDIENTE - Después de Fase 3)

- [ ] Actualizar README.md del proyecto
- [ ] Actualizar documentación de API (Swagger/OpenAPI)
- [ ] Documentar breaking changes
- [ ] Preparar release notes
- [ ] Notificar a consumidores de la API sobre cambios:
  - `/api/TodoLists/*` ya no existe
  - `POST /api/TodoItems` ya no requiere `listId`
- [ ] Actualizar guías de usuario si existen

---

## ?? Fase 6: Despliegue (PENDIENTE - Cuando esté listo para producción)

### Desarrollo/Staging
- [ ] Desplegar en ambiente de desarrollo
- [ ] Probar exhaustivamente
- [ ] Validar con equipo de QA
- [ ] Obtener aprobación

### Producción (cuando esté listo)
- [ ] **CRÍTICO**: Backup de producción
- [ ] Planificar ventana de mantenimiento
- [ ] Notificar a usuarios
- [ ] Desplegar código
- [ ] Ejecutar migraciones con `ApplyMigrations.sql`
- [ ] Verificar funcionalidad
- [ ] Monitorear logs y errores por 24-48 horas
- [ ] Tener plan de rollback listo (`RollbackMigrations.sql`)

---

## ? Verificación Post-Implementación

### Verificaciones Técnicas
- [ ] Aplicación inicia sin errores
- [ ] No hay errores de foreign key en logs
- [ ] Operaciones CRUD funcionan correctamente
- [ ] Performance es aceptable
- [ ] No hay memory leaks
- [ ] Logs no muestran advertencias inesperadas
- [ ] Endpoints `/api/TodoLists/*` retornan 404 (correcto)

### Verificaciones de Negocio
- [ ] Usuarios pueden crear items sin problemas
- [ ] Funcionalidad esperada está operativa
- [ ] No hay quejas de usuarios
- [ ] Métricas de uso son normales

---

## ?? Plan de Rollback (Si algo sale mal)

### Opción A: Restaurar desde Backup (Recomendado)
```sql
USE master;
ALTER DATABASE [FinalProjectDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [FinalProjectDb]
FROM DISK = 'C:\Backups\FinalProjectDb_PreMigration_20251021.bak'
WITH REPLACE;
ALTER DATABASE [FinalProjectDb] SET MULTI_USER;
```

### Opción B: Ejecutar Script de Rollback
- [ ] Abrir `RollbackMigrations.sql`
- [ ] Ejecutar script completo
- [ ] ?? NOTA: NO restaura datos, solo estructura

### Comunicación de Rollback
- [ ] Notificar a stakeholders
- [ ] Documentar razón del rollback
- [ ] Planificar nueva fecha de implementación

---

## ?? Métricas de Éxito

- [ ] ? 0 errores de foreign key constraint
- [ ] ? Tiempo de respuesta de API igual o mejor
- [ ] ? 0 errores críticos en producción
- [ ] ? Satisfacción de usuarios mantenida
- [ ] ? Tests pasan al 100%

---

## ?? Recursos y Referencias

### **Documentación Creada**:
- `README_CHANGES.md` - Resumen ejecutivo completo
- `MIGRATION_SUMMARY.md` - Análisis detallado de cambios
- `APPLY_MIGRATION_INSTRUCTIONS.md` - Instrucciones originales
- `CHECKLIST.md` - Este archivo (actualizado)
- **`ApplyMigrations.sql`** ? - Script SQL para aplicar migraciones
- **`RollbackMigrations.sql`** - Script SQL para rollback
- **`MIGRATION_GUIDE.md`** - Guía completa paso a paso

### **Archivos Modificados**:
- Ver lista completa en `README_CHANGES.md`

### **Migraciones**:
- `src/Infrastructure/Data/Migrations/20251021121625_RemoveTodoListDependency.cs`
- `src/Infrastructure/Data/Migrations/20251021122000_DropTodoListsTable.cs`

---

## ?? Estado Actual

**Fase Actual**: ?? Fase 2 - Migración de Base de Datos (LISTO PARA APLICAR)

**Completado**: 
- ? Fase 1: Modificación de Código (100%)
- ? Fase 4: Limpieza Completa de TodoList (100%)
- ? Scripts SQL creados y listos

**Pendiente**:
- ?? **Fase 2: Aplicar migraciones a base de datos** ? SIGUIENTE PASO
- ?? Fase 3: Pruebas (después de migración)
- ?? Fase 5: Documentación y comunicación
- ?? Fase 6: Despliegue a producción

**Próxima Acción INMEDIATA**: 
1. **Hacer backup de base de datos** ?? CRÍTICO
2. **Ejecutar `ApplyMigrations.sql`** en SQL Server Management Studio
3. **Verificar que migraciones se aplicaron correctamente**
4. **Probar aplicación**

---

## ?? Estimación de Tiempo

| Fase | Tiempo Estimado | Estado |
|------|-----------------|--------|
| Fase 1 (Código) | ? Completada | 100% |
| Fase 4 (Limpieza) | ? Completada | 100% |
| **Fase 2 (Migración)** | **15-30 minutos** | **? AHORA** |
| Fase 3 (Pruebas) | 1-2 horas | Pendiente |
| Fase 5 (Documentación) | 30-45 minutos | Pendiente |
| Fase 6 (Despliegue) | Variable | Pendiente |

---

**Última Actualización**: 2025-10-21
**Estado del Proyecto**: ? Código completo | ? Scripts SQL listos | ?? Migración pendiente de aplicar
**Responsable**: Copilot + Usuario
**Archivos Clave**: `ApplyMigrations.sql`, `MIGRATION_GUIDE.md`
