using Carter;
using eCommerce.UsersService.Api.Abstractions.Messaging;
using eCommerce.UsersService.Api.Contracts.Users;
using eCommerce.UsersService.Api.Database;
using eCommerce.UsersService.Api.Shared.Bases;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.UsersService.Api.Features.Users;

public class GetUsers
{
    #region Query
    public sealed class Query : IQuery<GetUsersResponse>
    {
    }
    #endregion

    #region Handler
    internal sealed class Handler(ApplicationDbContext context, 
        HandlerExecutor executor) : IQueryHandler<Query, GetUsersResponse>
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HandlerExecutor _executor = executor;

        public async Task<BaseResponse<GetUsersResponse>> Handle(Query query, 
            CancellationToken cancellationToken)
        {
            return await _executor.ExecuteAsync(
                query,
                () => GetUsersAsync(cancellationToken),
                cancellationToken
                );
        }

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
    public class GetUsersEndpoint : ICarterModule
    {
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