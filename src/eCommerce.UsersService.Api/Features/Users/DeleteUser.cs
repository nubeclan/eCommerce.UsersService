using Carter;
using eCommerce.UsersService.Api.Abstractions.Messaging;
using eCommerce.UsersService.Api.Database;
using eCommerce.UsersService.Api.Shared.Bases;

namespace eCommerce.UsersService.Api.Features.Users;

public class DeleteUser
{
    #region Command
    public sealed class Command : ICommand<bool>
    {
        public Guid UserID { get; set; }
    }
    #endregion

    #region Handler
    internal sealed class Handler(ApplicationDbContext context, 
        HandlerExecutor executor) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HandlerExecutor _executor = executor;

        public async Task<BaseResponse<bool>> Handle(Command command, 
            CancellationToken cancellationToken)
        {
            return await _executor.ExecuteAsync(
                command,
                () => DeleteUserAsync(command, cancellationToken),
                cancellationToken
                );
        }

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
    public class DeleteUserEndpoint : ICarterModule
    {
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