namespace Shared.DTO.User.Response;

public class GetUsersResponse : GenericResponse
{
    public Guid UserId { get; set; }
    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? TC { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

}
