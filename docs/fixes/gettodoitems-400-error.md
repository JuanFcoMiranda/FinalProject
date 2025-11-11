# Fix: Error 400 en GetTodoItemsEndpoint

## Problema Original

Al llamar al endpoint `GET /api/TodoItems` sin parámetros, se recibía un **Error 400 Bad Request**.

### Causa del Error

El validador `GetTodoItemsWithPaginationQueryValidator` requería que `ListId` fuera mayor que 0:

```csharp
RuleFor(x => x.ListId)
    .NotEmpty().WithMessage("ListId is required."); // ? Fallaba cuando ListId = 0
```

Cuando se llamaba al endpoint sin especificar `listId`, el parámetro se deserializaba como `0`, lo cual **fallaba la validación**.

## Solución Implementada

Se modificó el comportamiento para que **`ListId` sea opcional**:

### 1. Validador Actualizado

```csharp
RuleFor(x => x.ListId)
    .GreaterThanOrEqualTo(0).WithMessage("ListId must be greater than or equal to 0.");
 // ? Ahora acepta 0 (todos los items) o un ID específico
```

### 2. Handler Actualizado

```csharp
public async Task<PaginatedList<TodoItemBriefDto>> Handle(...)
{
    var query = context.TodoItems.AsQueryable();

    // Filter by ListId only if it's greater than 0
    if (request.ListId > 0)
    {
        query = query.Where(x => x.ListId == request.ListId);
    }

    return await query
        .OrderBy(x => x.Title)
        .ProjectTo<TodoItemBriefDto>(mapper.ConfigurationProvider)
        .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
}
```

## Comportamiento Actual

### Caso 1: Sin especificar ListId (devuelve TODOS los items)
```http
GET /api/TodoItems
```
? Devuelve items de **todas las listas** con paginación por defecto (página 1, 10 items)

### Caso 2: Con ListId específico
```http
GET /api/TodoItems?listId=5
```
? Devuelve solo items de la lista con ID = 5

### Caso 3: Con paginación personalizada
```http
GET /api/TodoItems?listId=5&pageNumber=2&pageSize=20
```
? Devuelve items de la lista 5, página 2, 20 items por página

### Caso 4: Todos los items con paginación
```http
GET /api/TodoItems?pageNumber=1&pageSize=50
```
? Devuelve primeros 50 items de todas las listas

## Parámetros del Endpoint

| Parámetro | Tipo | Requerido | Valor por Defecto | Descripción |
|-----------|------|-----------|-------------------|-------------|
| `listId` | int | ? No | 0 | ID de la lista. Si es 0, devuelve items de todas las listas |
| `pageNumber` | int | ? No | 1 | Número de página |
| `pageSize` | int | ? No | 10 | Cantidad de items por página |

## Validaciones Actuales

? `listId >= 0` (0 = todas las listas, >0 = lista específica)  
? `pageNumber >= 1`  
? `pageSize >= 1`

## Ejemplos de Uso

### JavaScript/Fetch
```javascript
// Obtener todos los items (primera página)
fetch('http://localhost:5000/api/TodoItems')
  .then(res => res.json())
  .then(data => console.log(data));

// Obtener items de una lista específica
fetch('http://localhost:5000/api/TodoItems?listId=5')
  .then(res => res.json())
  .then(data => console.log(data));

// Paginación personalizada
fetch('http://localhost:5000/api/TodoItems?listId=5&pageNumber=2&pageSize=20')
  .then(res => res.json())
  .then(data => console.log(data));
```

### Angular HttpClient
```typescript
getTodoItems(listId?: number, pageNumber: number = 1, pageSize: number = 10) {
  let params = new HttpParams()
    .set('pageNumber', pageNumber.toString())
    .set('pageSize', pageSize.toString());
  
  if (listId && listId > 0) {
    params = params.set('listId', listId.toString());
  }
  
  return this.http.get<PaginatedList<TodoItemBriefDto>>(
    `${this.apiUrl}/TodoItems`,
    { params }
  );
}
```

### C# HttpClient
```csharp
// Todos los items
var response = await httpClient.GetAsync("/api/TodoItems");

// Lista específica con paginación
var response = await httpClient.GetAsync(
    "/api/TodoItems?listId=5&pageNumber=1&pageSize=20"
);
```

## Respuesta del Endpoint

```json
{
  "items": [
{
    "id": 1,
      "listId": 5,
      "title": "Comprar leche",
      "done": false
    },
    {
   "id": 2,
      "listId": 5,
      "title": "Estudiar para examen",
      "done": true
    }
  ],
  "pageNumber": 1,
  "totalPages": 3,
  "totalCount": 25,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## Retrocompatibilidad

?? **Cambio de Comportamiento:**
- **Antes:** Era obligatorio especificar `listId` (error 400 si no se proporcionaba)
- **Ahora:** `listId` es opcional (devuelve todos los items si no se especifica)

Si necesitas mantener el comportamiento anterior (requerir `listId`), puedes:

1. Revertir el validador a:
```csharp
RuleFor(x => x.ListId)
 .NotEmpty().WithMessage("ListId is required.");
```

2. Revertir el handler a:
```csharp
return await context.TodoItems
    .Where(x => x.ListId == request.ListId)
    .OrderBy(x => x.Title)
    .ProjectTo<TodoItemBriefDto>(mapper.ConfigurationProvider)
    .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
```

## Archivos Modificados

- ? `src/Application/TodoItems/Queries/GetTodoItemsWithPagination/GetTodoItemsWithPaginationQueryValidator.cs`
- ? `src/Application/TodoItems/Queries/GetTodoItemsWithPagination/GetTodoItemsWithPagination.cs`

## Testing

Para probar el endpoint:

```bash
# Sin parámetros (todos los items)
curl http://localhost:5000/api/TodoItems

# Con listId específico
curl "http://localhost:5000/api/TodoItems?listId=5"

# Con paginación
curl "http://localhost:5000/api/TodoItems?listId=5&pageNumber=2&pageSize=20"

# Todos los items con paginación
curl "http://localhost:5000/api/TodoItems?pageNumber=1&pageSize=50"
```
