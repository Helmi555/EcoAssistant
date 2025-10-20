using BCrypt.Net;
using EcoAssistant.Application.DTOs;
using EcoAssistant.Application.Interfaces;
using EcoAssistant.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcoAssistant.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IConfiguration _config;

    public UserService(IUserRepository repo, IConfiguration config)
        => (_repo, _config) = (repo, config);

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

    public async Task<string> LoginAsync(string username, string password, CancellationToken ct = default)
    {
            var user = await _repo.GetByUsernameAsync(username, ct) ?? throw new InvalidOperationException("user_not_found");

        if (!VerifyPassword(user, password))
        throw new InvalidOperationException("invalid_password"); 

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
        var jwtExpireDaysStr = _config["Jwt:ExpireDays"] ?? throw new InvalidOperationException("Jwt:ExpireDays not configured");
        if (!int.TryParse(jwtExpireDaysStr, out var expireDays))
            throw new InvalidOperationException("Jwt:ExpireDays is not a valid integer");
        var issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer not configured");
        var audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience not configured");

        var key = Encoding.ASCII.GetBytes(jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username)
            ]),
            Expires = DateTime.UtcNow.AddDays(expireDays),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool VerifyPassword(User user, string password) =>
        BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

    private static UserDto ToDto(User u) =>
        new(u.Id, u.Name, u.LastName, u.Username, u.CreatedAt, u.UpdatedAt, u.DateOfBirth, u.Address);
}
