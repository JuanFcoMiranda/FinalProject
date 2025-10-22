# Configuraci�n de CORS

## Descripci�n

Se ha configurado CORS (Cross-Origin Resource Sharing) en la aplicaci�n para permitir peticiones desde el frontend que corre en `localhost:4200`.

## Configuraci�n Implementada

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

**Importante:** El middleware de CORS debe estar ubicado despu�s de `UseHttpsRedirection()` y antes de `UseFastEndpoints()` para funcionar correctamente.

## Caracter�sticas de la Pol�tica CORS

| Configuraci�n | Descripci�n |
|---------------|-------------|
| **Origins** | `http://localhost:4200` y `https://localhost:4200` |
| **Headers** | Permite cualquier header (`.AllowAnyHeader()`) |
| **Methods** | Permite cualquier m�todo HTTP: GET, POST, PUT, DELETE, etc. (`.AllowAnyMethod()`) |
| **Credentials** | Permite env�o de credenciales como cookies y headers de autenticaci�n (`.AllowCredentials()`) |

## Uso desde el Frontend

### Angular (HttpClient)

```typescript
import { HttpClient } from '@angular/common/http';

export class ApiService {
  private apiUrl = 'http://localhost:5000/api'; // o el puerto de tu API

  constructor(private http: HttpClient) {}

  getTodoLists() {
    return this.http.get(`${this.apiUrl}/TodoLists`, {
      withCredentials: true // Necesario si usas autenticaci�n
    });
  }
}
```

### Fetch API

```javascript
fetch('http://localhost:5000/api/TodoLists', {
  method: 'GET',
  credentials: 'include', // Necesario si usas autenticaci�n
  headers: {
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => console.log(data));
```

## Consideraciones de Seguridad

### Para Desarrollo
La configuraci�n actual es apropiada para desarrollo local.

### Para Producci�n
**IMPORTANTE:** Para producci�n, debes:

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

2. **Limitar m�todos HTTP** si es necesario:
```csharp
policy.WithMethods("GET", "POST", "PUT", "DELETE")
```

3. **Limitar headers** si es posible:
```csharp
policy.WithHeaders("Content-Type", "Authorization")
```

4. **Usar configuraci�n desde appsettings.json**:

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

## Pol�ticas M�ltiples (Opcional)

Si necesitas diferentes pol�ticas para diferentes escenarios:

```csharp
builder.Services.AddCors(options =>
{
    // Pol�tica para desarrollo
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    // Pol�tica para producci�n
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
1. El origen no est� en la lista de or�genes permitidos
2. `UseCors()` est� en el orden incorrecto en el pipeline
3. Falta la configuraci�n de credenciales en el cliente

**Soluci�n:**
1. Verificar que el origen est� correctamente configurado
2. Asegurar que `UseCors()` est� despu�s de `UseHttpsRedirection()` y antes de los endpoints
3. Agregar `withCredentials: true` o `credentials: 'include'` en las peticiones del cliente

### Error: "Credentials flag is true, but Access-Control-Allow-Credentials is not"

**Causa:** El servidor est� configurado con `AllowCredentials()` pero el cliente no est� enviando credenciales.

**Soluci�n:** Agregar `withCredentials: true` en las peticiones HTTP del cliente.

## Referencias

- [Microsoft Docs - Enable CORS in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/cors)
- [MDN Web Docs - CORS](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS)
