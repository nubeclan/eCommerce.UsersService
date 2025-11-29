using System.Collections.Generic;

namespace eCommerce.UsersService.Api.Contracts.Users;

public record GetUsersResponse(
    List<GetUserResponse> Users);