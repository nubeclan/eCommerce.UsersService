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