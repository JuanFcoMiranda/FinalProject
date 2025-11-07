# ?? Documentación de Configuración

Guías sobre configuración del proyecto, incluyendo CORS, autenticación y autorización.

## ?? Documentos Disponibles

### Autenticación y Autorización
- **[auth.md](auth.md)** - Guía completa de autenticación y autorización
- **[auth-quick-reference.md](auth-quick-reference.md)** - Referencia rápida de configuración de auth

### CORS (Cross-Origin Resource Sharing)
- **[cors.md](cors.md)** - Configuración de CORS para frontend

## ?? Configuraciones Principales

### Autenticación

El proyecto usa ASP.NET Core Identity con soporte para:
- ? Autenticación basada en cookies
- ? JWT tokens (preparado pero no habilitado)
- ? Roles y Claims
- ? Identity UI (opcional)

**Estado**: ? Configurado pero no enforced (usa `AllowAnonymous`)

Ver [auth.md](auth.md) para activar autenticación.

### CORS

Configurado para permitir requests desde:
- `http://localhost:4200` (Angular development)
- `https://localhost:4200` (Angular development SSL)

**Características**:
- ? Cualquier header permitido
- ? Cualquier método HTTP permitido
- ? Credenciales habilitadas

Ver [cors.md](cors.md) para detalles.

## ?? Inicio Rápido

### Activar Autenticación

```csharp
// En tu endpoint
public override void Configure()
{
    Get("/api/secure-endpoint");
    RequireAuthorization();  // ? Cambia de AllowAnonymous() a esto
}
```

### Configurar CORS para Nuevo Origen

```csharp
// En Program.cs
app.UseCors(policy => policy
    .WithOrigins("http://localhost:4200", "https://miapp.com")  // ? Agrega aquí
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
```

## ?? Configuraciones Adicionales

### appsettings.json

```json
{
  "ConnectionStrings": {
    "FinalProjectDb": "..."
  },
  "Logging": {...},
  "AllowedHosts": "*",
  "Jwt": {  // Si usas JWT
    "Key": "tu-secret-key-aqui",
    "Issuer": "tu-issuer",
    "Audience": "tu-audience"
  }
}
```

### Variables de Entorno

Para producción, usa variables de entorno en lugar de valores hardcodeados:

```bash
# Docker Compose
environment:
  - JWT_KEY=${JWT_KEY}
  - DB_PASSWORD=${DB_PASSWORD}
```

## ?? Mejores Prácticas de Seguridad

### Autenticación
- ? Usa contraseñas fuertes (requerimiento de Identity)
- ? Habilita 2FA para usuarios admin
- ? Usa HTTPS en producción
- ? Rota secretos regularmente

### CORS
- ?? No uses `AllowAnyOrigin()` en producción
- ?? Especifica orígenes exactos
- ?? No combines `AllowAnyOrigin()` con `AllowCredentials()`

### Secretos
- ? Usa Azure Key Vault o similar en producción
- ? No commitees secretos en Git
- ? Usa User Secrets para desarrollo local
- ? Rota secrets después de exposición

## ??? Comandos Útiles

### User Secrets (Desarrollo Local)

```bash
# Inicializar user secrets
dotnet user-secrets init --project src/Web

# Agregar secret
dotnet user-secrets set "Jwt:Key" "mi-secret-key" --project src/Web

# Listar secrets
dotnet user-secrets list --project src/Web

# Remover secret
dotnet user-secrets remove "Jwt:Key" --project src/Web
```

### Verificar Configuración

```bash
# Ver configuración cargada
dotnet run --project src/Web --urls "http://localhost:5000"

# En otra terminal, probar CORS
curl -H "Origin: http://localhost:4200" -I http://localhost:5000/api/TodoItems
```

## ?? Enlaces Relacionados

- [Documentación Principal](../README.md)
- [Docker](../docker/)
- [Base de Datos](../database/)

## ?? Recursos Externos

- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [CORS in ASP.NET Core](https://docs.microsoft.com/aspnet/core/security/cors)
- [JWT Authentication](https://jwt.io/introduction)
- [Azure Key Vault](https://azure.microsoft.com/services/key-vault/)

---

**Última actualización**: Enero 2025
