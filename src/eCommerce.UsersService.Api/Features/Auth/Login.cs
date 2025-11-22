using Carter;
using eCommerce.UsersService.Api.Abstractions.Messaging;
using eCommerce.UsersService.Api.Contracts.Auth;
using eCommerce.UsersService.Api.Database;
using eCommerce.UsersService.Api.Entities;
using eCommerce.UsersService.Api.Shared.Bases;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace eCommerce.UsersService.Api.Features.Auth;

public class Login
{
    #region Command
    public sealed class Command : ICommand<LoginResponse>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    #endregion

    #region Validator
    public class Validator : AbstractValidator<Command>
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
    internal sealed class Handler(ApplicationDbContext context, 
        HandlerExecutor executor,
        IConfiguration configuration) : ICommandHandler<Command, LoginResponse>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HandlerExecutor _executor = executor;
        private readonly IConfiguration _configuration = configuration;

        public async Task<BaseResponse<LoginResponse>> Handle(Command command, 
            CancellationToken cancellationToken)
        {
            return await _executor.ExecuteAsync(
                command,
                () => LoginAsync(command, cancellationToken),
                cancellationToken
                );
        }

        private async Task<BaseResponse<LoginResponse>> LoginAsync(Command command,
            CancellationToken cancellationToken)
        {
            var response = new BaseResponse<LoginResponse>();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);
                if (user == null || !BC.Verify(command.Password, user.Password))
                {
                    response.IsSuccess = false;
                    response.Message = "Credenciales inválidas.";
                    return response;
                }

                var token = GenerateJwtToken(user);
                response.Data = new LoginResponse(token, user.Email, user.FirstName, user.LastName);
                response.IsSuccess = true;
                response.Message = "Inicio de sesión exitoso.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al iniciar sesión: {ex.Message}";
            }

            return response;
        }

        private string GenerateJwtToken(Entities.User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    #endregion

    #region Endpoint
    public class LoginEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/auth/login", async (
                    LoginRequest request,
                    IDispatcher dispatcher,
                    CancellationToken cancellationToken
                ) =>
            {
                var command = request.Adapt<Command>();
                var response = await dispatcher
                .Dispatch<Command, LoginResponse>(command, cancellationToken);

                return response.IsSuccess ? Results.Ok(response) : Results.Unauthorized();
            });
        }
    }
    #endregion
}