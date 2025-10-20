namespace EcoAssistant.API.Dtos;

public record CreateUserRequest(string Name, string LastName, string Username, string Password, DateTime? DateOfBirth = null, string? Address = null);
