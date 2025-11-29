using Carter;
using eCommerce.UsersService.Api.Abstractions.Messaging;
using eCommerce.UsersService.Api.Database;
using eCommerce.UsersService.Api.Shared.Bases;

/**
 * Funcionalidad para eliminar un usuario
 * 
 * Implementa el comando para eliminar un usuario existente del sistema.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */
namespace eCommerce.UsersService.Api.Features.Users;

public class DeleteUser
{
    #region Command
    /// <summary>
    /// Comando para eliminar un usuario.
    /// </summary>
    public sealed class Command : ICommand<bool>
    {
        /// <summary>
        /// Identificador único del usuario a eliminar.
        /// </summary>
        public Guid UserID { get; set; }
    }
    #endregion

    #region Handler
    /// <summary>
    /// Manejador para el comando de eliminar usuario.
    /// </summary>
    internal sealed class Handler(ApplicationDbContext context, 
        HandlerExecutor executor) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HandlerExecutor _executor = executor;

        /// <summary>
        /// Maneja el comando de eliminar usuario.
        /// </summary>
        /// <param name="command">Comando con el ID del usuario.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con el resultado de la operación.</returns>
        public async Task<BaseResponse<bool>> Handle(Command command, 
            CancellationToken cancellationToken)
        {
            return await _executor.ExecuteAsync(
                command,
                () => DeleteUserAsync(command, cancellationToken),
                cancellationToken
                );
        }

        /// <summary>
        /// Elimina el usuario de forma asíncrona.
        /// </summary>
        /// <param name="command">Comando con el ID.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con el resultado.</returns>
        private async Task<BaseResponse<bool>> DeleteUserAsync(Command command,
            CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var user = await _context.Users.FindAsync(command.UserID, cancellationToken);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Usuario no encontrado.";
                    return response;
                }

                _context.Users.Remove(user);
                var recordsAffected = await _context.SaveChangesAsync(cancellationToken);

                response.IsSuccess = true;
                response.Message = "Usuario eliminado correctamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al eliminar usuario: {ex.Message}";
            }

            return response;
        }
    }
    #endregion

    #region Endpoint
    /// <summary>
    /// Endpoint para eliminar un usuario.
    /// </summary>
    public class DeleteUserEndpoint : ICarterModule
    {
        /// <summary>
        /// Configura las rutas para el endpoint.
        /// </summary>
        /// <param name="app">Constructor de rutas de endpoint.</param>
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/users/{userId:guid}", async (
                    Guid userId,
                    IDispatcher dispatcher,
                    CancellationToken cancellationToken
                ) =>
            {
                var command = new Command { UserID = userId };
                var response = await dispatcher
                .Dispatch<Command, bool>(command, cancellationToken);

                return response.IsSuccess ? Results.Ok(response) : Results.NotFound(response);
            });
        }
    }
    #endregion
}