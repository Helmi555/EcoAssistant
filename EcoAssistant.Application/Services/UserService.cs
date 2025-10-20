using BCrypt.Net;
using EcoAssistant.Application.DTOs;
using EcoAssistant.Application.Interfaces;
using EcoAssistant.Domain.Entities;

namespace EcoAssistant.Application.Services;

public class UserService
{
    private readonly IUserRepository _repo;
    public UserService(IUserRepository repo) => _repo = repo;

    public async Task<UserDto> CreateAsync(string name, string lastName, string username, string password, DateTime? dob = null, string? address = null, CancellationToken ct = default)
    {
        if (await _repo.GetByUsernameAsync(username, ct) is not null)
            throw new InvalidOperationException("username_taken");

        var user = new User
        {
            Name = name,
            LastName = lastName,
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            DateOfBirth = dob,
            Address = address,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        user = await _repo.AddAsync(user, ct);
        return ToDto(user);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var u = await _repo.GetByIdAsync(id, ct);
        return u is null ? null : ToDto(u);
    }

    public async Task<List<UserDto>> ListAsync(int limit = 50, int offset = 0, CancellationToken ct = default)
    {
        var users = await _repo.ListAsync(limit, offset, ct);
        return users.Select(ToDto).ToList();
    }

    private static UserDto ToDto(User u) =>
        new(u.Id, u.Name, u.LastName, u.Username, u.CreatedAt, u.UpdatedAt, u.DateOfBirth, u.Address);

    // Password verify helper
    public bool VerifyPassword(User user, string password) =>
        BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
}
