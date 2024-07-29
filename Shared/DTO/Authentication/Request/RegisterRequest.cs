namespace Shared.DTO.Authentication.Request;

public class RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
}
