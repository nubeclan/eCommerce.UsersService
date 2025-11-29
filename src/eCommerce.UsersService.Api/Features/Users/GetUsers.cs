using Carter;
using eCommerce.UsersService.Api.Abstractions.Messaging;
using eCommerce.UsersService.Api.Contracts.Users;
using eCommerce.UsersService.Api.Database;
using eCommerce.UsersService.Api.Shared.Bases;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.UsersService.Api.Features.Users;

/**
 * Funcionalidad para obtener todos los usuarios
 * 
 * Implementa la query para obtener una lista de todos los usuarios.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */
public class GetUsers
{
    #region Query
    /// <summary>
    /// Query para obtener todos los usuarios.
    /// </summary>
    public sealed class Query : IQuery<GetUsersResponse>
    {
    }
    #endregion

    #region Handler
    /// <summary>
    /// Manejador para la query de obtener usuarios.
    /// </summary>
    internal sealed class Handler(ApplicationDbContext context, 
        HandlerExecutor executor) : IQueryHandler<Query, GetUsersResponse>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HandlerExecutor _executor = executor;

        /// <summary>
        /// Maneja la query de obtener usuarios.
        /// </summary>
        /// <param name="query">Query vacía.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con la lista de usuarios.</returns>
        public async Task<BaseResponse<GetUsersResponse>> Handle(Query query, 
            CancellationToken cancellationToken)
        {
            return await _executor.ExecuteAsync(
                query,
                () => GetUsersAsync(cancellationToken),
                cancellationToken
                );
        }

        /// <summary>
        /// Obtiene la lista de usuarios de forma asíncrona.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con la lista de usuarios.</returns>
        private async Task<BaseResponse<GetUsersResponse>> GetUsersAsync(
            CancellationToken cancellationToken)
        {
            var response = new BaseResponse<GetUsersResponse>();

            try
            {
                var users = await _context.Users.ToListAsync(cancellationToken);
                var userResponses = users.Adapt<List<GetUserResponse>>();
                response.Data = new GetUsersResponse(userResponses);
                response.IsSuccess = true;
                response.Message = "Usuarios obtenidos correctamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al obtener usuarios: {ex.Message}";
            }

            return response;
        }
    }
    #endregion

    #region Endpoint
    /// <summary>
    /// Endpoint para obtener todos los usuarios.
    /// </summary>
    public class GetUsersEndpoint : ICarterModule
    {
        /// <summary>
        /// Configura las rutas para el endpoint.
        /// </summary>
        /// <param name="app">Constructor de rutas de endpoint.</param>
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/users", async (
                    IDispatcher dispatcher,
                    CancellationToken cancellationToken
                ) =>
            {
                var query = new Query();
                var response = await dispatcher
                .Dispatch<Query, GetUsersResponse>(query, cancellationToken);

                return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
            });
        }
    }
    #endregion
}