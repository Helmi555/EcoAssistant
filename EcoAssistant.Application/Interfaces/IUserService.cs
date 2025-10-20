using EcoAssistant.Application.DTOs;
using EcoAssistant.Domain.Entities;

public interface IUserService
{
    Task<UserDto> CreateAsync(string name, string lastName, string username, string password, DateTime? dob = null, string? address = null, CancellationToken ct = default);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<UserDto>> ListAsync(int limit = 50, int offset = 0, CancellationToken ct = default);
    Task<string> LoginAsync(string username, string password, CancellationToken ct = default);
    bool VerifyPassword(User user, string password);
}
