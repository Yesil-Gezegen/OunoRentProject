namespace Shared.DTO.User.Request;

public sealed record CreateUserRequest(
    string Email,
    string PasswordHash);

