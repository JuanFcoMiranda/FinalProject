# Configuraci�n de Autenticaci�n y Autorizaci�n

## Estado Actual

? **Autenticaci�n y Autorizaci�n HABILITADAS** en el pipeline  
? **Todos los endpoints son P�BLICOS por defecto** (`AllowAnonymous()`)  
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

| M�todo | Endpoint | Descripci�n |
|--------|----------|-------------|
| POST | `/identity/register` | Registrar nuevo usuario |
| POST | `/identity/login` | Iniciar sesi�n (obtener token) |
| POST | `/identity/refresh` | Refrescar token de acceso |
| POST | `/identity/logout` | Cerrar sesi�n |
| GET | `/identity/confirmEmail` | Confirmar email |
| POST | `/identity/resendConfirmationEmail` | Reenviar email de confirmaci�n |
| POST | `/identity/forgotPassword` | Solicitar reset de contrase�a |
| POST | `/identity/resetPassword` | Resetear contrase�a |
| POST | `/identity/manage/2fa` | Configurar autenticaci�n de dos factores |
| GET | `/identity/manage/info` | Obtener informaci�n del usuario |
| POST | `/identity/manage/info` | Actualizar informaci�n del usuario |

## Endpoints Actuales (Todos P�blicos)

Todos los endpoints FastEndpoints est�n configurados con `AllowAnonymous()`:

```csharp
public override void Configure()
{
    Get("/api/TodoLists");
    AllowAnonymous(); // ? P�blico por defecto
}
```

### Lista de Endpoints P�blicos

**TodoLists:**
- ? GET `/api/TodoLists` - P�blico
- ? POST `/api/TodoLists` - P�blico
- ? PUT `/api/TodoLists/{id}` - P�blico
- ? DELETE `/api/TodoLists/{id}` - P�blico

**TodoItems:**
- ? GET `/api/TodoItems` - P�blico
- ? POST `/api/TodoItems` - P�blico
- ? PUT `/api/TodoItems/{id}` - P�blico
- ? PUT `/api/TodoItems/UpdateDetail/{id}` - P�blico
- ? DELETE `/api/TodoItems/{id}` - P�blico

**WeatherForecasts:**
- ? GET `/api/WeatherForecasts` - P�blico

## C�mo Proteger Endpoints (Cuando lo Necesites)

### Opci�n 1: Requerir Autenticaci�n (Cualquier Usuario Logueado)

Simplemente remueve `AllowAnonymous()` o usa `Policies()`:

```csharp
public override void Configure()
{
    Get("/api/TodoLists");
    // Remover AllowAnonymous() hace que requiera autenticaci�n
}
```

### Opci�n 2: Requerir Roles Espec�ficos

```csharp
public override void Configure()
{
    Delete("/api/TodoLists/{id}");
    Roles("Administrator"); // Solo administradores
}
```

### Opci�n 3: Requerir Pol�ticas Espec�ficas

```csharp
public override void Configure()
{
    Delete("/api/TodoLists/{id}");
    Policies("CanPurge"); // Requiere la pol�tica CanPurge
}
```

### Opci�n 4: M�ltiples Roles

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

### 2. Inicio de Sesi�n

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

## Ejemplo: Servicio de Autenticaci�n en Angular

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

## Roles y Pol�ticas Disponibles

### Roles (Definidos en Domain/Constants/Roles.cs)

```csharp
public static class Roles
{
    public const string Administrator = nameof(Administrator);
}
```

### Pol�ticas (Definidas en Domain/Constants/Policies.cs)

```csharp
public static class Policies
{
    public const string CanPurge = nameof(CanPurge);
}
```

La pol�tica `CanPurge` est� configurada para requerir el rol `Administrator`.

## Crear Nuevos Roles y Pol�ticas

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

### Agregar una Nueva Pol�tica

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

## Migraci�n de Endpoints: De P�blico a Protegido

### Antes (P�blico):
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

### Despu�s (Solo Administradores):
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

## Testing de Autenticaci�n

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

## Pr�ximos Pasos Recomendados

1. ? **Infraestructura lista** - Autenticaci�n y autorizaci�n habilitadas
2. ? **Endpoints p�blicos** - Por defecto sin protecci�n
3. ?? **Cuando necesites proteger un endpoint** - Cambia `AllowAnonymous()` por `Roles()` o `Policies()`
4. ?? **Seed inicial** - Crear usuario administrador por defecto
5. ?? **Email confirmation** - Configurar servicio de email para confirmaci�n
6. ?? **Password policies** - Configurar requisitos de contrase�a
7. ?? **Token expiration** - Ajustar tiempos de expiraci�n de tokens

## Referencias

- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Bearer Token Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization)
- [FastEndpoints Authorization](https://fast-endpoints.com/docs/security)
