# ? Checklists y Registros

Listas de verificación y registros de cambios del proyecto.

## ?? Documentos Disponibles

### Checklists
- **[project-checklist.md](project-checklist.md)** - Lista de tareas y verificaciones del proyecto

### Registros de Cambios
- **[readme-changes.md](readme-changes.md)** - Registro de cambios en documentación

## ?? Tipos de Checklists

### Pre-Commit Checklist
Antes de hacer commit:
- [ ] Código compila sin errores
- [ ] Tests pasan
- [ ] Código formateado (EditorConfig)
- [ ] Sin warnings importantes
- [ ] Documentación actualizada

### Pre-PR Checklist
Antes de crear Pull Request:
- [ ] Tests completos ejecutados
- [ ] Cobertura de código mantenida/mejorada
- [ ] Documentación actualizada
- [ ] README actualizado si es necesario
- [ ] CHANGELOG actualizado
- [ ] Screenshots/demos si aplica

### Pre-Release Checklist
Antes de hacer release:
- [ ] Todos los tests pasan
- [ ] Builds de producción exitosos
- [ ] Documentación completa
- [ ] CHANGELOG finalizado
- [ ] Versión actualizada en archivos de proyecto
- [ ] Tags de Git creados
- [ ] Docker images construidas y testeadas

### Pre-Deployment Checklist
Antes de desplegar:
- [ ] Backup de base de datos
- [ ] Variables de entorno configuradas
- [ ] Secretos configurados
- [ ] Health checks funcionando
- [ ] Monitoring configurado
- [ ] Logs configurados
- [ ] Plan de rollback preparado

## ?? Registro de Cambios

### Formato de CHANGELOG

```markdown
# Changelog

## [Unreleased]
### Added
- Nueva funcionalidad X

### Changed
- Mejorada funcionalidad Y

### Fixed
- Corregido bug Z

## [1.0.0] - 2025-01-XX
### Added
- Funcionalidad inicial
```

## ??? Herramientas

### Generación Automática de CHANGELOG

```bash
# Con conventional-changelog
npm install -g conventional-changelog-cli
conventional-changelog -p angular -i CHANGELOG.md -s
```

### Versionado Semántico

Formato: `MAJOR.MINOR.PATCH`

- **MAJOR**: Cambios incompatibles con API anterior
- **MINOR**: Nueva funcionalidad compatible con versiones anteriores
- **PATCH**: Correcciones de bugs compatibles

Ejemplos:
- `1.0.0` ? `1.0.1` (bug fix)
- `1.0.1` ? `1.1.0` (nueva feature)
- `1.1.0` ? `2.0.0` (breaking change)

## ?? Templates

### Template de Checklist

```markdown
# Checklist: [Nombre de la Tarea]

## Prerequisitos
- [ ] Item 1
- [ ] Item 2

## Tareas Principales
- [ ] Tarea 1
  - [ ] Subtarea 1.1
  - [ ] Subtarea 1.2
- [ ] Tarea 2

## Verificación
- [ ] Verificación 1
- [ ] Verificación 2

## Post-Tarea
- [ ] Documentar
- [ ] Comunicar
```

## ?? Enlaces Relacionados

- [Documentación Principal](../README.md)
- [Migración](../migration/)
- [Fixes](../fixes/)

---

**Última actualización**: Enero 2025
