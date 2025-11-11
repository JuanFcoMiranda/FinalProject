# ?? Documentación de Docker

Guías completas sobre el uso de Docker, contenedores y despliegue del proyecto FinalProject.

## ?? Documentos Disponibles

### ?? Inicio Rápido
- **[quickstart.md](quickstart.md)** - Guía rápida para levantar el proyecto con Docker en minutos

### ?? Guías Completas
- **[overview.md](overview.md)** - Visión general de la arquitectura Docker del proyecto
- **[dockerfile-explained.md](dockerfile-explained.md)** - Explicación detallada del Dockerfile multi-stage

### ?? Solución de Problemas
- **[error-solution.md](error-solution.md)** - Soluciones a errores comunes de Docker
- **[database-container-fix.md](database-container-fix.md)** - Cómo se resolvió el problema del contenedor SQL Server

### ? Optimización
- **[size-optimization.md](size-optimization.md)** - Estrategias avanzadas de optimización de imágenes
- **[size-quick-guide.md](size-quick-guide.md)** - Guía rápida de optimización
- **[test-results.md](test-results.md)** - Resultados de pruebas de diferentes configuraciones

## ?? Por Dónde Empezar

### Si eres nuevo en el proyecto
?? Empieza con **[quickstart.md](quickstart.md)**

### Si tienes problemas
?? Revisa **[error-solution.md](error-solution.md)** y **[database-container-fix.md](database-container-fix.md)**

### Si quieres entender cómo funciona
?? Lee **[overview.md](overview.md)** y **[dockerfile-explained.md](dockerfile-explained.md)**

### Si quieres optimizar
?? Consulta **[size-optimization.md](size-optimization.md)** y **[size-quick-guide.md](size-quick-guide.md)**

## ?? Comandos Rápidos

```bash
# Levantar todo el stack
docker-compose up -d

# Ver logs
docker-compose logs -f

# Ver estado
docker-compose ps

# Reiniciar
docker-compose restart

# Detener
docker-compose down

# Reiniciar desde cero (elimina volúmenes)
docker-compose down -v
```

## ?? Problemas Comunes

### El contenedor SQL Server no inicia
? Ver [database-container-fix.md](database-container-fix.md)

### Error "Cannot open database"
? Ver [error-solution.md](error-solution.md) sección "Solución al Error 'Cannot open database'"

### La imagen es muy grande
? Ver [size-quick-guide.md](size-quick-guide.md)

### La aplicación no responde
? Ver [quickstart.md](quickstart.md) sección "Verificación"

## ?? Estructura de Contenedores

```yaml
services:
  sqlserver:        # SQL Server 2022
    - Puerto: 1433
    - Health check: Cada 10s
    
  webapp:           # Aplicación .NET 9
    - Puerto: 8080
    - Dependencia: sqlserver
    - Auto-restart: yes
```

## ?? Configuración de Seguridad

?? **IMPORTANTE**: Las contraseñas por defecto son para desarrollo local.

**En producción**:
- Cambia todas las contraseñas
- Usa variables de entorno o secretos
- No expongas puertos innecesarios
- Usa HTTPS

Ver más en [overview.md](overview.md#seguridad)

## ?? Enlaces Relacionados

- [Documentación de Base de Datos](../database/)
- [Documentación de Testing](../testing/)
- [README Principal](../README.md)

---

**Última actualización**: Enero 2025
