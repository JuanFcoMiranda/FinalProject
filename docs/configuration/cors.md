# Configuración de CORS

## Descripción

Se ha configurado CORS (Cross-Origin Resource Sharing) en la aplicación para permitir peticiones desde el frontend que corre en `localhost:4200`.

## Configuración Implementada

### 1. Servicios de CORS (DependencyInjection.cs)

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

### 2. Middleware de CORS (Program.cs)

```csharp
app.UseCors();
```

**Importante:** El middleware de CORS debe estar ubicado después de `UseHttpsRedirection()` y antes de `UseFastEndpoints()` para funcionar correctamente.

## Características de la Política CORS

| Configuración | Descripción |
|---------------|-------------|
| **Origins** | `http://localhost:4200` y `https://localhost:4200` |
| **Headers** | Permite cualquier header (`.AllowAnyHeader()`) |
| **Methods** | Permite cualquier método HTTP: GET, POST, PUT, DELETE, etc. (`.AllowAnyMethod()`) |
| **Credentials** | Permite envío de credenciales como cookies y headers de autenticación (`.AllowCredentials()`) |

## Uso desde el Frontend

### Angular (HttpClient)

```typescript
import { HttpClient } from '@angular/common/http';

export class ApiService {
  private apiUrl = 'http://localhost:5000/api'; // o el puerto de tu API

  constructor(private http: HttpClient) {}

  getTodoLists() {
    return this.http.get(`${this.apiUrl}/TodoLists`, {
      withCredentials: true // Necesario si usas autenticación
    });
  }
}
```

### Fetch API

```javascript
fetch('http://localhost:5000/api/TodoLists', {
  method: 'GET',
  credentials: 'include', // Necesario si usas autenticación
  headers: {
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => console.log(data));
```

## Consideraciones de Seguridad

### Para Desarrollo
La configuración actual es apropiada para desarrollo local.

### Para Producción
**IMPORTANTE:** Para producción, debes:

1. **Especificar dominios exactos** en lugar de usar `localhost`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://tuapp.com",
            "https://www.tuapp.com"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
```

2. **Limitar métodos HTTP** si es necesario:
```csharp
policy.WithMethods("GET", "POST", "PUT", "DELETE")
```

3. **Limitar headers** si es posible:
```csharp
policy.WithHeaders("Content-Type", "Authorization")
```

4. **Usar configuración desde appsettings.json**:

**appsettings.json:**
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://tuapp.com",
      "https://www.tuapp.com"
    ]
  }
}
```

**DependencyInjection.cs:**
```csharp
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

## Políticas Múltiples (Opcional)

Si necesitas diferentes políticas para diferentes escenarios:

```csharp
builder.Services.AddCors(options =>
{
    // Política para desarrollo
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    // Política para producción
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://tuapp.com")
              .WithHeaders("Content-Type", "Authorization")
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .AllowCredentials();
    });
});
```

**En Program.cs:**
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
}
else
{
    app.UseCors("Production");
}
```

## Troubleshooting

### Error: "Access to fetch has been blocked by CORS policy"

**Posibles causas:**
1. El origen no está en la lista de orígenes permitidos
2. `UseCors()` está en el orden incorrecto en el pipeline
3. Falta la configuración de credenciales en el cliente

**Solución:**
1. Verificar que el origen esté correctamente configurado
2. Asegurar que `UseCors()` esté después de `UseHttpsRedirection()` y antes de los endpoints
3. Agregar `withCredentials: true` o `credentials: 'include'` en las peticiones del cliente

### Error: "Credentials flag is true, but Access-Control-Allow-Credentials is not"

**Causa:** El servidor está configurado con `AllowCredentials()` pero el cliente no está enviando credenciales.

**Solución:** Agregar `withCredentials: true` en las peticiones HTTP del cliente.

## Referencias

- [Microsoft Docs - Enable CORS in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/cors)
- [MDN Web Docs - CORS](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS)
