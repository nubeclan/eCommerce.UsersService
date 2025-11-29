using Carter;
using eCommerce.UsersService.Api.Abstractions.Messaging;
using eCommerce.UsersService.Api.Contracts.Users;
using eCommerce.UsersService.Api.Database;
using eCommerce.UsersService.Api.Entities;
using eCommerce.UsersService.Api.Shared.Bases;
using FluentValidation;
using Mapster;

/**
 * Funcionalidad para actualizar un usuario
 * 
 * Implementa el comando para actualizar los datos de un usuario existente.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */
public class UpdateUser
{
    #region Command
    /// <summary>
    /// Comando para actualizar un usuario.
    /// </summary>
    public sealed class Command : ICommand<bool>
    {
        /// <summary>
        /// Identificador único del usuario a actualizar.
        /// </summary>
        public Guid UserID { get; set; }
        /// <summary>
        /// Nuevo nombre del usuario.
        /// </summary>
        public string FirstName { get; set; } = null!;
        /// <summary>
        /// Nuevo apellido del usuario.
        /// </summary>
        public string LastName { get; set; } = null!;
        /// <summary>
        /// Nuevo correo electrónico del usuario.
        /// </summary>
        public string Email { get; set; } = null!;
    }
    #endregion

    #region Validator
    /// <summary>
    /// Validador para el comando de actualizar usuario.
    /// </summary>
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .NotNull()
                .WithMessage("El nombre no puede ser nulo ni vacío.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .NotNull()
                .WithMessage("El apellido no puede ser nulo ni vacío.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress()
                .WithMessage("El email debe ser válido.");
        }
    }
    #endregion

    #region Handler
    /// <summary>
    /// Manejador para el comando de actualizar usuario.
    /// </summary>
    internal sealed class Handler(ApplicationDbContext context, 
        HandlerExecutor executor) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HandlerExecutor _executor = executor;

        /// <summary>
        /// Maneja el comando de actualizar usuario.
        /// </summary>
        /// <param name="command">Comando con los nuevos datos del usuario.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con el resultado de la operación.</returns>
        public async Task<BaseResponse<bool>> Handle(Command command, 
            CancellationToken cancellationToken)
        {
            return await _executor.ExecuteAsync(
                command,
                () => UpdateUserAsync(command, cancellationToken),
                cancellationToken
                );
        }

        /// <summary>
        /// Actualiza el usuario de forma asíncrona.
        /// </summary>
        /// <param name="command">Comando con los nuevos datos.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con el resultado.</returns>
        private async Task<BaseResponse<bool>> UpdateUserAsync(Command command,
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

                user.FirstName = command.FirstName;
                user.LastName = command.LastName;
                user.Email = command.Email;

                _context.Users.Update(user);
                var recordsAffected = await _context.SaveChangesAsync(cancellationToken);

                response.IsSuccess = true;
                response.Message = "Usuario actualizado correctamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al actualizar usuario: {ex.Message}";
            }

            return response;
        }
    }
    #endregion

    #region Endpoint
    /// <summary>
    /// Endpoint para actualizar un usuario.
    /// </summary>
    public class UpdateUserEndpoint : ICarterModule
    {
        /// <summary>
        /// Configura las rutas para el endpoint.
        /// </summary>
        /// <param name="app">Constructor de rutas de endpoint.</param>
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/users/{userId:guid}", async (
                    Guid userId,
                    UpdateUserRequest request,
                    IDispatcher dispatcher,
                    CancellationToken cancellationToken
                ) =>
            {
                var command = request.Adapt<Command>();
                command.UserID = userId;
                var response = await dispatcher
                .Dispatch<Command, bool>(command, cancellationToken);

                return response.IsSuccess ? Results.Ok(response) : Results.BadRequest(response);
            });
        }
    }
    #endregion
}