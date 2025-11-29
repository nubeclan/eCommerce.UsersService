namespace eCommerce.UsersService.Api.Contracts.Users;

public record GetUserResponse(
    Guid UserID,
    string FirstName,
    string LastName,
    string Email);