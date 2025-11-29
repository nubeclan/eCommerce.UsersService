using Carter;
using eCommerce.UsersService.Api.Abstractions.Messaging;
using eCommerce.UsersService.Api.Contracts.Users;
using eCommerce.UsersService.Api.Database;
using eCommerce.UsersService.Api.Shared.Bases;
using Mapster;

namespace eCommerce.UsersService.Api.Features.Users;

/**
 * Funcionalidad para obtener un usuario
 * 
 * Implementa la query para obtener los datos de un usuario específico.
 * 
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */
public class GetUser
{
    #region Query
    /// <summary>
    /// Query para obtener un usuario.
    /// </summary>
    public sealed class Query : IQuery<GetUserResponse>
    {
        /// <summary>
        /// Identificador único del usuario a obtener.
        /// </summary>
        public Guid UserID { get; set; }
    }
    #endregion

    #region Handler
    /// <summary>
    /// Manejador para la query de obtener usuario.
    /// </summary>
    internal sealed class Handler(ApplicationDbContext context, 
        HandlerExecutor executor) : IQueryHandler<Query, GetUserResponse>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HandlerExecutor _executor = executor;

        /// <summary>
        /// Maneja la query de obtener usuario.
        /// </summary>
        /// <param name="query">Query con el ID del usuario.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con los datos del usuario.</returns>
        public async Task<BaseResponse<GetUserResponse>> Handle(Query query, 
            CancellationToken cancellationToken)
        {
            return await _executor.ExecuteAsync(
                query,
                () => GetUserAsync(query, cancellationToken),
                cancellationToken
                );
        }

        /// <summary>
        /// Obtiene el usuario de forma asíncrona.
        /// </summary>
        /// <param name="query">Query con el ID.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con los datos del usuario.</returns>
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
    /// <summary>
    /// Endpoint para obtener un usuario.
    /// </summary>
    public class GetUserEndpoint : ICarterModule
    {
        /// <summary>
        /// Configura las rutas para el endpoint.
        /// </summary>
        /// <param name="app">Constructor de rutas de endpoint.</param>
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