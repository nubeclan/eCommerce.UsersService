# Guía para Añadir Funcionalidades API REST

Para añadir nuevas funcionalidades REST, sigue el patrón existente descrito en `README.md`. Crea una nueva clase en la carpeta `Features` correspondiente (e.g., `Users` para usuarios, `Auth` para autenticación).

### Funcionalidades Ya Implementadas
El proyecto ya incluye un CRUD completo para usuarios y autenticación JWT:

- **Registro de Usuario**: `POST /api/users/register`
- **Login**: `POST /api/auth/login`
- **Obtener Todos los Usuarios**: `GET /api/users`
- **Obtener Usuario por ID**: `GET /api/users/{userId}`
- **Actualizar Usuario**: `PUT /api/users/{userId}`
- **Eliminar Usuario**: `DELETE /api/users/{userId}`

Puedes revisar las implementaciones en `Features/Users` y `Features/Auth` como ejemplos.

### Pasos Generales para Añadir Nuevas Funcionalidades
1. **Define el Contrato (DTO)**: Crea un record en `Contracts` para el request/response.
2. **Crea la Entidad si es necesario**: Actualiza `Entities` y `ApplicationDbContext`.
3. **Implementa la Funcionalidad**: Crea una clase en `Features` con Command/Query, Validator, Handler y Endpoint.
4. **Configura Mapeos**: Usa Mapster para mapear DTOs a comandos/entidades.
5. **Ejecuta Migraciones**: Si cambias la DB, crea y aplica migraciones EF.
6. **Prueba**: Ejecuta el proyecto y verifica con Swagger/OpenAPI.

### Ejemplo: Añadir Obtener Usuario por ID (Query) - Ya Implementado
Para referencia, aquí está la implementación existente en `Features\Users\GetUser.cs`:

```csharp
using Carter;
using eCommerce.UsersService.Api.Abstractions.Messaging;
using eCommerce.UsersService.Api.Contracts.Users;
using eCommerce.UsersService.Api.Database;
using eCommerce.UsersService.Api.Shared.Bases;
using Mapster;

namespace eCommerce.UsersService.Api.Features.Users;

public class GetUser
{
    #region Query
    public sealed class Query : IQuery<GetUserResponse>
    {
        public Guid UserID { get; set; }
    }
    #endregion

    #region Handler
    internal sealed class Handler(ApplicationDbContext context, 
        HandlerExecutor executor) : IQueryHandler<Query, GetUserResponse>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HandlerExecutor _executor = executor;

        public async Task<BaseResponse<GetUserResponse>> Handle(Query query, 
            CancellationToken cancellationToken)
        {
            return await _executor.ExecuteAsync(
                query,
                () => GetUserAsync(query, cancellationToken),
                cancellationToken
                );
        }

        private async Task<BaseResponse<GetUserResponse>> GetUserAsync(Query query,
            CancellationToken cancellationToken)
        {
            var response = new BaseResponse<GetUserResponse>();

            try
            {
                var user = await _context.Users.FindAsync(query.UserID, cancellationToken);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Usuario no encontrado.";
                    return response;
                }

                response.Data = user.Adapt<GetUserResponse>();
                response.IsSuccess = true;
                response.Message = "Usuario obtenido correctamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al obtener usuario: {ex.Message}";
            }

            return response;
        }
    }
    #endregion

    #region Endpoint
    public class GetUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/users/{userId:guid}", async (
                    Guid userId,
                    IDispatcher dispatcher,
                    CancellationToken cancellationToken
                ) =>
            {
                var query = new Query { UserID = userId };
                var response = await dispatcher
                .Dispatch<Query, GetUserResponse>(query, cancellationToken);

                return response.IsSuccess ? Results.Ok(response) : Results.NotFound(response);
            });
        }
    }
    #endregion
}
```

#### 3. Configura el Mapeo en `Program.cs` o un archivo de configuración
Mapster escanea automáticamente, pero asegúrate de que el mapeo de `User` a `GetUserResponse` esté configurado si es necesario.

#### 4. Ejecuta y Prueba
- Restaura paquetes: `dotnet restore`
- Aplica migraciones: `dotnet ef database update`
- Ejecuta: `dotnet run`
- Accede a Swagger: `https://localhost:XXXX/swagger` y prueba `GET /api/users/{userId}`

### Notas Adicionales
- Para operaciones de escritura (Commands), usa `ICommandHandler` y endpoints POST/PUT/DELETE.
- Para lectura (Queries), usa `IQueryHandler` y GET.
- Siempre valida inputs con FluentValidation.
- Maneja errores consistentemente con `BaseResponse`.
- Si añades nuevas entidades, actualiza `ApplicationDbContext` y crea configuraciones en `Database\Configurations`.
- Para autenticación, expande `Features\Auth` con JWT (ya implementado).

Esta guía mantiene la consistencia con la arquitectura actual, facilitando la escalabilidad del microservicio.

## Desarrollador

*  [Angel Céspedes Quiroz](https://bo.linkedin.com/in/acq1305)
*  Correo:  <angel@nubeando.com>