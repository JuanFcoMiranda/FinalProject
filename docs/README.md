# ?? Documentación del Proyecto

Documentación técnica organizada por temáticas para el proyecto FinalProject.

> ?? **Resumen de Organización**: Ver [organization-summary.md](organization-summary.md) para detalles sobre cómo está estructurada la documentación.

## ?? Estructura de Documentación

### ?? [Docker](docker/)
Guías y documentación relacionada con Docker, contenedores y despliegue.

- [**Inicio Rápido**](docker/quickstart.md) - Guía rápida para levantar el proyecto con Docker
- [**Visión General**](docker/overview.md) - Información general sobre Docker en el proyecto
- [**Solución de Errores**](docker/error-solution.md) - Cómo resolver errores comunes de Docker
- [**Fix del Contenedor de BD**](docker/database-container-fix.md) - Solución al problema del contenedor SQL Server
- [**Explicación del Dockerfile**](docker/dockerfile-explained.md) - Detalles sobre cómo funciona el Dockerfile
- [**Optimización de Tamaño**](docker/size-optimization.md) - Estrategias para reducir el tamaño de las imágenes
- [**Guía Rápida de Tamaño**](docker/size-quick-guide.md) - Referencia rápida de optimización
- [**Resultados de Tests**](docker/test-results.md) - Resultados de pruebas de las imágenes Docker

### ?? [Base de Datos](database/)
Documentación sobre configuración, migración y gestión de la base de datos.

- [**Migración a SQL Server**](database/sql-server-migration.md) - Guía completa de migración desde LocalDB
- [**Fix de Conexión**](database/connection-fix.md) - Solución a problemas de conexión

### ?? [Testing](testing/)
Guías y documentación sobre pruebas y cobertura de código.

- [**README de Cobertura**](testing/coverage-readme.md) - Introducción a la cobertura de tests
- [**Guía de Ejecución**](testing/run-coverage-guide.md) - Cómo ejecutar análisis de cobertura
- [**Visión General**](testing/coverage-overview.md) - Detalles sobre la cobertura del proyecto
- [**Resumen de Cobertura**](testing/coverage-summary.md) - Resumen de métricas de cobertura

### ?? [Migración](migration/)
Documentación sobre migraciones y actualizaciones del proyecto.

- [**Guía de FastEndpoints**](migration/fastendpoints-guide.md) - Migración a FastEndpoints
- [**Resumen de FastEndpoints**](migration/fastendpoints-summary.md) - Resumen de la migración
- [**Migración Completa**](migration/fastendpoints-full.md) - Documentación completa de migración
- [**Instrucciones de Aplicación**](migration/apply-instructions.md) - Cómo aplicar migraciones

### ?? [Configuración](configuration/)
Guías de configuración del proyecto.

- [**CORS**](configuration/cors.md) - Configuración de CORS
- [**Autenticación**](configuration/auth.md) - Autenticación y autorización
- [**Referencia Rápida de Auth**](configuration/auth-quick-reference.md) - Referencia rápida

### ?? [Fixes](fixes/)
Soluciones a problemas específicos encontrados.

- [**Error 400 en GetTodoItems**](fixes/gettodoitems-400-error.md) - Solución al error 400

### ? [Checklists](checklists/)
Listas de verificación y cambios del proyecto.

- [**Checklist del Proyecto**](checklists/project-checklist.md) - Lista de tareas y verificaciones
- [**Cambios en README**](checklists/readme-changes.md) - Registro de cambios en documentación

## ?? Inicio Rápido

### Para Desarrolladores Nuevos

1. **Configurar el Entorno**
   ```bash
   # Clonar el repositorio
   git clone https://github.com/JuanFcoMiranda/FinalProject
   cd FinalProject
   ```

2. **Levantar con Docker** (Recomendado)
   ```bash
   # Leer primero
   docs/docker/quickstart.md
   
   # Ejecutar
   docker-compose up -d
   ```

3. **Desarrollo Local**
   ```bash
   # Restaurar dependencias
   dotnet restore
   
   # Ejecutar
   cd src/Web
   dotnet run
   ```

### Para Problemas Comunes

- **Error de contenedor SQL Server**: Ver [docker/database-container-fix.md](docker/database-container-fix.md)
- **Error de conexión a BD**: Ver [database/connection-fix.md](database/connection-fix.md)
- **Problemas con Docker**: Ver [docker/error-solution.md](docker/error-solution.md)
- **Error 400 en API**: Ver [fixes/gettodoitems-400-error.md](fixes/gettodoitems-400-error.md)

## ?? Convenciones de Documentación

### Nomenclatura de Archivos
- Usar **kebab-case** para nombres de archivos (ej: `database-container-fix.md`)
- Nombres descriptivos y concisos
- Sin espacios ni caracteres especiales

### Estructura de Documentos
Cada documento debe incluir:
- **Título claro** con emoji descriptivo
- **Tabla de contenidos** (para docs largos)
- **Ejemplos de código** con syntax highlighting
- **Referencias cruzadas** a otros documentos relacionados
- **Fecha de última actualización** (opcional)

### Uso de Emojis
- ?? Para guías de inicio rápido
- ? Para soluciones exitosas
- ?? Para advertencias
- ?? Para bugs y problemas
- ?? Para listas y pasos
- ?? Para tips y consejos
- ?? Para configuración
- ?? Para métricas y estadísticas

## ?? Mantenimiento

### Actualizar Documentación

Cuando modifiques el código:
1. Actualiza la documentación relevante
2. Verifica enlaces rotos
3. Actualiza ejemplos de código
4. Revisa que los comandos funcionen

### Agregar Nueva Documentación

1. Coloca el archivo en la carpeta temática apropiada
2. Actualiza este README con el enlace
3. Actualiza el README de la carpeta temática
4. Sigue las convenciones de nomenclatura
5. Agrega referencias cruzadas desde documentos relacionados

## ?? Plantillas

### Plantilla de Documento Técnico

```markdown
# ?? Título del Documento

Breve descripción del propósito del documento.

## Tabla de Contenidos
- [Prerequisitos](#prerequisitos)
- [Paso a Paso](#paso-a-paso)
- [Solución de Problemas](#solución-de-problemas)
- [Referencias](#referencias)

## Prerequisitos
- Lista de requisitos

## Paso a Paso
1. Primer paso
2. Segundo paso

## Solución de Problemas
### Problema 1
- Descripción
- Solución

## Referencias
- [Documento relacionado 1](...)
- [Documento relacionado 2](...)
```

## ??? Herramientas de Documentación

### Visualizar Markdown
- **VS Code**: Instala extensión "Markdown All in One"
- **Online**: [StackEdit](https://stackedit.io/), [Dillinger](https://dillinger.io/)

### Verificar Enlaces
```bash
# Con markdown-link-check
npm install -g markdown-link-check
markdown-link-check docs/**/*.md
```

### Generar Tabla de Contenidos
```bash
# Con doctoc
npm install -g doctoc
doctoc docs/**/*.md
```

## ?? Contacto y Contribución

¿Encontraste un error en la documentación? ¿Tienes una sugerencia?
- Abre un issue en GitHub
- Envía un pull request con correcciones
- Contacta al equipo de desarrollo

## ?? Estadísticas de Documentación

| Categoría | Documentos | Estado |
|-----------|------------|--------|
| Docker | 8 | ? Completo |
| Base de Datos | 2 | ? Completo |
| Testing | 4 | ? Completo |
| Migración | 4 | ? Completo |
| Configuración | 3 | ? Completo |
| Fixes | 1 | ? Actualizado |
| Checklists | 2 | ? Actualizado |
| **Total** | **24** | **? Organizado** |

## ?? Historial de Cambios

| Fecha | Cambio | Autor |
|-------|--------|-------|
| 2025-01-07 | Reorganización completa de documentación | Copilot |
| 2025-01-07 | Creación de estructura temática | Copilot |
| 2025-01-07 | Agregadas categorías: migración, configuración, fixes, checklists | Copilot |

---

**Última actualización**: Enero 2025
