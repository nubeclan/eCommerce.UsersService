using Carter;
using eCommerce.UsersService.Api.Abstractions.Messaging;
using eCommerce.UsersService.Api.Configurations.Authentication;
using eCommerce.UsersService.Api.Contracts.Users;
using eCommerce.UsersService.Api.Database;
using eCommerce.UsersService.Api.Shared.Bases;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace eCommerce.UsersService.Api.Features.Auth;

/**
 * Funcionalidad para iniciar sesión
 * 
 * Implementa la query para autenticar un usuario y generar un token JWT.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */
public class Login
{
    #region Query
    /// <summary>
    /// Query para iniciar sesión.
    /// </summary>
    public sealed class Query : IQuery<AuthenticationResponse>
    {
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
    /// Validador para la query de login.
    /// </summary>
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress()
                .WithMessage("El email debe ser válido.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .NotNull()
                .WithMessage("La contraseña no puede ser nula ni vacía.");
        }
    }
    #endregion

    #region Handler
    /// <summary>
    /// Manejador para la query de login.
    /// </summary>
    internal sealed class Handler(
        ApplicationDbContext context,
        HandlerExecutor executor,
        IJwtTokenGenerator jwtTokenGenerator)
        : IQueryHandler<Query, AuthenticationResponse>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HandlerExecutor _executor = executor;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

        /// <summary>
        /// Maneja la query de login.
        /// </summary>
        /// <param name="query">Query con las credenciales.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con el token JWT.</returns>
        public async Task<BaseResponse<AuthenticationResponse>> Handle(
            Query query, CancellationToken cancellationToken)
        {
            return await _executor.ExecuteAsync(
                query,
                () => LoginAsync(query, cancellationToken),
                cancellationToken);
        }

        /// <summary>
        /// Autentica al usuario de forma asíncrona.
        /// </summary>
        /// <param name="query">Query con las credenciales.</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Respuesta con el token.</returns>
        private async Task<BaseResponse<AuthenticationResponse>> LoginAsync(
            Query query, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<AuthenticationResponse>();

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(
                    x => x.Email == query.Email, cancellationToken);

                if (user is null)
                {
                    response.IsSuccess = false;
                    response.Message = "El usuario no existe en la base de datos.";
                    return response;
                }

                if (!BC.Verify(query.Password, user.Password))
                {
                    response.IsSuccess = false;
                    response.Message = "La contraseña es incorrecta.";
                    return response;
                }

                var token = _jwtTokenGenerator.GenerateToken(user);
                var authToken = new AuthenticationResponse(token, user.Email, user.FirstName, user.LastName);

                response.IsSuccess = true;
                response.Data = authToken;
                response.Message = "Token generado correctamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Ocurrió un error al procesar la solicitud: {ex.Message}";
            }

            return response;
        }
    }
    #endregion

    #region Endpoint
    /// <summary>
    /// Endpoint para iniciar sesión.
    /// </summary>
    public class LoginEndpoint : ICarterModule
    {
        /// <summary>
        /// Configura las rutas para el endpoint.
        /// </summary>
        /// <param name="app">Constructor de rutas de endpoint.</param>
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/auth/login", async (
                    LoginRequest request,
                    IDispatcher dispatcher,
                    CancellationToken cancellationToken) =>
            {
                var query = request.Adapt<Query>();
                var response = await dispatcher
                .Dispatch<Query, AuthenticationResponse>(query, cancellationToken);
                return Results.Ok(response);
            })
            .WithName("Login")
            .WithTags("Authentication")
            .WithSummary("Iniciar sesión")
            .WithDescription("Autentica un usuario y genera un token JWT para acceder a los recursos protegidos del sistema.")
            .Produces<BaseResponse<AuthenticationResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
    #endregion
}
