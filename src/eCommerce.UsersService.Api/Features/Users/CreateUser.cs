using Carter;
using eCommerce.UsersService.Api.Abstractions.Messaging;
using eCommerce.UsersService.Api.Contracts.Users;
using eCommerce.UsersService.Api.Database;
using eCommerce.UsersService.Api.Entities;
using eCommerce.UsersService.Api.Shared.Bases;
using FluentValidation;
using Mapster;
using BC = BCrypt.Net.BCrypt;

namespace eCommerce.UsersService.Api.Features.Users;

/**
 * Funcionalidad para crear un usuario
 * 
 * Implementa el comando para registrar un nuevo usuario en el sistema.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */
public class CreateUser
{
    #region Command
    /// <summary>
    /// Comando para crear un usuario.
    /// </summary>
    public sealed class Command : ICommand<bool>
    {
        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string FirstName { get; set; } = null!;
        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        public string LastName { get; set; } = null!;
        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        public string Password { get; set; } = null!;
    }
    #endregion

    #region Validator
    /// <summary>
    /// Validador para el comando de crear usuario.
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
    /// Manejador para el comando de crear usuario.
    /// </summary>
    internal sealed class Handler(ApplicationDbContext context, 
        HandlerExecutor executor) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HandlerExecutor _executor = executor;

        /// <summary>
        /// Maneja el comando de crear usuario.
        /// </summary>
        /// <param name="command">Comando con los datos del usuario.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con el resultado de la operación.</returns>
        public async Task<BaseResponse<bool>> Handle(Command command, 
            CancellationToken cancellationToken)
        {
            return await _executor.ExecuteAsync(
                command,
                () => CreateUserAsync(command, cancellationToken),
                cancellationToken
                );
        }

        /// <summary>
        /// Crea el usuario de forma asíncrona.
        /// </summary>
        /// <param name="command">Comando con los datos.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con el resultado.</returns>
        private async Task<BaseResponse<bool>> CreateUserAsync(Command command,
            CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var user = command.Adapt<User>();
                user.Password = BC.EnhancedHashPassword(command.Password);

                _context.Add(user);

                var recordsAffected = await _context.SaveChangesAsync(cancellationToken);

                response.IsSuccess = true;
                response.Message = "Se registró correctamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Ocurrió un error al registrar el usuario: {ex.Message}";
            }

            return response;
        }
    }
    #endregion

    #region Endpoint
    /// <summary>
    /// Endpoint para registrar un usuario.
    /// </summary>
    public class CreateUserEndpoint : ICarterModule
    {
        /// <summary>
        /// Configura las rutas para el endpoint.
        /// </summary>
        /// <param name="app">Constructor de rutas de endpoint.</param>
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/users/register", async (
                    RegisterRequest request,
                    IDispatcher dispatcher,
                    CancellationToken cancellationToken
                ) =>
            {
                var command = request.Adapt<Command>();
                var response = await dispatcher
                .Dispatch<Command, bool>(command, cancellationToken);

                return Results.Ok(response);
            })
            .WithName("RegisterUser")
            .WithTags("Users")
            .WithSummary("Registrar un nuevo usuario")
            .WithDescription("Crea un nuevo usuario en el sistema con los datos proporcionados. La contraseña será encriptada automáticamente.")
            .Produces<BaseResponse<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
    #endregion
}
