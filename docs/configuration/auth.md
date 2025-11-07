# Configuración de Autenticación y Autorización

## Estado Actual

? **Autenticación y Autorización HABILITADAS** en el pipeline  
? **Todos los endpoints son PÚBLICOS por defecto** (`AllowAnonymous()`)  
? **ASP.NET Core Identity API** disponible en `/identity/*`

## Infraestructura Implementada

### 1. Servicios Configurados (Infrastructure/DependencyInjection.cs)

```csharp
builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorizationBuilder();

builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();
```

### 2. Middleware Activado (Program.cs)

```csharp
app.UseAuthentication();  // ? Habilitado
app.UseAuthorization();   // ? Habilitado
```

### 3. Identity API Endpoints

Se han mapeado los endpoints de Identity en `/identity/*`:

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/identity/register` | Registrar nuevo usuario |
| POST | `/identity/login` | Iniciar sesión (obtener token) |
| POST | `/identity/refresh` | Refrescar token de acceso |
| POST | `/identity/logout` | Cerrar sesión |
| GET | `/identity/confirmEmail` | Confirmar email |
| POST | `/identity/resendConfirmationEmail` | Reenviar email de confirmación |
| POST | `/identity/forgotPassword` | Solicitar reset de contraseña |
| POST | `/identity/resetPassword` | Resetear contraseña |
| POST | `/identity/manage/2fa` | Configurar autenticación de dos factores |
| GET | `/identity/manage/info` | Obtener información del usuario |
| POST | `/identity/manage/info` | Actualizar información del usuario |

## Endpoints Actuales (Todos Públicos)

Todos los endpoints FastEndpoints están configurados con `AllowAnonymous()`:

```csharp
public override void Configure()
{
    Get("/api/TodoLists");
    AllowAnonymous(); // ? Público por defecto
}
```

### Lista de Endpoints Públicos

**TodoLists:**
- ? GET `/api/TodoLists` - Público
- ? POST `/api/TodoLists` - Público
- ? PUT `/api/TodoLists/{id}` - Público
- ? DELETE `/api/TodoLists/{id}` - Público

**TodoItems:**
- ? GET `/api/TodoItems` - Público
- ? POST `/api/TodoItems` - Público
- ? PUT `/api/TodoItems/{id}` - Público
- ? PUT `/api/TodoItems/UpdateDetail/{id}` - Público
- ? DELETE `/api/TodoItems/{id}` - Público

**WeatherForecasts:**
- ? GET `/api/WeatherForecasts` - Público

## Cómo Proteger Endpoints (Cuando lo Necesites)

### Opción 1: Requerir Autenticación (Cualquier Usuario Logueado)

Simplemente remueve `AllowAnonymous()` o usa `Policies()`:

```csharp
public override void Configure()
{
    Get("/api/TodoLists");
    // Remover AllowAnonymous() hace que requiera autenticación
}
```

### Opción 2: Requerir Roles Específicos

```csharp
public override void Configure()
{
    Delete("/api/TodoLists/{id}");
    Roles("Administrator"); // Solo administradores
}
```

### Opción 3: Requerir Políticas Específicas

```csharp
public override void Configure()
{
    Delete("/api/TodoLists/{id}");
    Policies("CanPurge"); // Requiere la política CanPurge
}
```

### Opción 4: Múltiples Roles

```csharp
public override void Configure()
{
    Post("/api/TodoItems");
    Roles("Administrator", "PowerUser"); // Cualquiera de estos roles
}
```

## Uso desde el Cliente

### 1. Registro de Usuario

```typescript
const registerUser = async (email: string, password: string) => {
  const response = await fetch('http://localhost:5000/identity/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      email: email,
      password: password
    })
  });
  
  if (response.ok) {
    return await response.json();
  }
  throw new Error('Registration failed');
};
```

### 2. Inicio de Sesión

```typescript
const login = async (email: string, password: string) => {
  const response = await fetch('http://localhost:5000/identity/login?useCookies=false', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      email: email,
      password: password
    })
  });
  
  if (response.ok) {
    const data = await response.json();
    // data.accessToken contiene el Bearer token
    // data.refreshToken para renovar el token
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    return data;
  }
  throw new Error('Login failed');
};
```

### 3. Usar el Token en Peticiones

```typescript
const getTodoLists = async () => {
  const token = localStorage.getItem('accessToken');
  
  const response = await fetch('http://localhost:5000/api/TodoLists', {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
  
  return await response.json();
};
```

### 4. Refrescar Token (Cuando Expira)

```typescript
const refreshToken = async () => {
  const refresh = localStorage.getItem('refreshToken');
  
  const response = await fetch('http://localhost:5000/identity/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      refreshToken: refresh
    })
  });
  
  if (response.ok) {
    const data = await response.json();
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    return data;
  }
  throw new Error('Token refresh failed');
};
```

## Ejemplo: Servicio de Autenticación en Angular

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5000/identity';
  private tokenSubject = new BehaviorSubject<string | null>(
    localStorage.getItem('accessToken')
  );
  
  public token$ = this.tokenSubject.asObservable();

  constructor(private http: HttpClient) {}

  register(email: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, { email, password });
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this.apiUrl}/login?useCookies=false`,
      { email, password }
    ).pipe(
      tap(response => {
        localStorage.setItem('accessToken', response.accessToken);
        localStorage.setItem('refreshToken', response.refreshToken);
        this.tokenSubject.next(response.accessToken);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.tokenSubject.next(null);
    
    // Opcional: llamar al endpoint de logout
    this.http.post(`${this.apiUrl}/logout`, {}).subscribe();
  }

  getToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  refreshToken(): Observable<AuthResponse> {
    const refresh = localStorage.getItem('refreshToken');
    return this.http.post<AuthResponse>(
      `${this.apiUrl}/refresh`,
      { refreshToken: refresh }
    ).pipe(
      tap(response => {
        localStorage.setItem('accessToken', response.accessToken);
        localStorage.setItem('refreshToken', response.refreshToken);
        this.tokenSubject.next(response.accessToken);
      })
    );
  }
}
```

## HTTP Interceptor para Angular

```typescript
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, switchMap, filter, take } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.authService.getToken();
    
    if (token) {
      request = this.addToken(request, token);
    }

    return next.handle(request).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse && error.status === 401) {
          return this.handle401Error(request, next);
        }
        return throwError(() => error);
      })
    );
  }

  private addToken(request: HttpRequest<any>, token: string) {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((response: any) => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(response.accessToken);
          return next.handle(this.addToken(request, response.accessToken));
        }),
        catchError((err) => {
          this.isRefreshing = false;
          this.authService.logout();
          return throwError(() => err);
        })
      );
    } else {
      return this.refreshTokenSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap(token => {
          return next.handle(this.addToken(request, token));
        })
      );
    }
  }
}
```

## Roles y Políticas Disponibles

### Roles (Definidos en Domain/Constants/Roles.cs)

```csharp
public static class Roles
{
    public const string Administrator = nameof(Administrator);
}
```

### Políticas (Definidas en Domain/Constants/Policies.cs)

```csharp
public static class Policies
{
    public const string CanPurge = nameof(CanPurge);
}
```

La política `CanPurge` está configurada para requerir el rol `Administrator`.

## Crear Nuevos Roles y Políticas

### Agregar un Nuevo Rol

1. **Definir en Domain/Constants/Roles.cs:**
```csharp
public static class Roles
{
    public const string Administrator = nameof(Administrator);
    public const string PowerUser = nameof(PowerUser);     // ? Nuevo
    public const string RegularUser = nameof(RegularUser); // ? Nuevo
}
```

2. **Agregar en Infrastructure/Data/ApplicationDbContextInitialiser.cs:**
```csharp
private async Task SeedRolesAsync()
{
    if (!await _roleManager.RoleExistsAsync(Roles.Administrator))
    {
        await _roleManager.CreateAsync(new IdentityRole(Roles.Administrator));
    }
    
    if (!await _roleManager.RoleExistsAsync(Roles.PowerUser))
    {
        await _roleManager.CreateAsync(new IdentityRole(Roles.PowerUser));
    }
}
```

### Agregar una Nueva Política

**En Infrastructure/DependencyInjection.cs:**
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.CanPurge, 
        policy => policy.RequireRole(Roles.Administrator));
    
    options.AddPolicy("CanEdit", 
        policy => policy.RequireRole(Roles.Administrator, Roles.PowerUser));
    
    options.AddPolicy("CanView", 
        policy => policy.RequireAuthenticatedUser());
});
```

## Migración de Endpoints: De Público a Protegido

### Antes (Público):
```csharp
public class DeleteTodoListEndpoint : Endpoint<DeleteTodoListRequest>
{
    public override void Configure()
    {
        Delete("/api/TodoLists/{id}");
        AllowAnonymous(); // ? Cualquiera puede eliminar
    }
}
```

### Después (Solo Administradores):
```csharp
public class DeleteTodoListEndpoint : Endpoint<DeleteTodoListRequest>
{
    public override void Configure()
    {
        Delete("/api/TodoLists/{id}");
        Roles("Administrator"); // ? Solo administradores
    }
}
```

## Testing de Autenticación

### Obtener un Token para Testing

```bash
# 1. Registrar usuario
curl -X POST http://localhost:5000/identity/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!"}'

# 2. Login
curl -X POST http://localhost:5000/identity/login?useCookies=false \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!"}'

# 3. Usar el token
curl -X GET http://localhost:5000/api/TodoLists \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN_HERE"
```

## Próximos Pasos Recomendados

1. ? **Infraestructura lista** - Autenticación y autorización habilitadas
2. ? **Endpoints públicos** - Por defecto sin protección
3. ?? **Cuando necesites proteger un endpoint** - Cambia `AllowAnonymous()` por `Roles()` o `Policies()`
4. ?? **Seed inicial** - Crear usuario administrador por defecto
5. ?? **Email confirmation** - Configurar servicio de email para confirmación
6. ?? **Password policies** - Configurar requisitos de contraseña
7. ?? **Token expiration** - Ajustar tiempos de expiración de tokens

## Referencias

- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Bearer Token Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization)
- [FastEndpoints Authorization](https://fast-endpoints.com/docs/security)
