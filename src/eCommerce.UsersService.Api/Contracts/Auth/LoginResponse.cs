namespace eCommerce.UsersService.Api.Contracts.Auth;

public record LoginResponse(
    string Token,
    string Email,
    string FirstName,
    string LastName);