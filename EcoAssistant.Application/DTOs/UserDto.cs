namespace EcoAssistant.Application.DTOs;

public record UserDto(Guid Id, string Name, string LastName, string Username, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTime? DateOfBirth, string? Address);

public class UserLoginRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
