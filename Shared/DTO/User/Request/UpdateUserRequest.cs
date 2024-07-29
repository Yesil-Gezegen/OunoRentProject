namespace Shared.DTO.User.Request;

public sealed record UpdateUserRequest(
    Guid Id,
    string Name,
    string Surname,
    string Email,
    string PhoneNumber,
    string Address,
    string Tc,
    string Gender,
    DateTime BirthDate
);

