namespace AC.Api.Endpoints.Users.CreateUser;

public class CreateUserRequest
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Dni { get; set; }
    public Guid RoleId { get; set; }
}