using System;

namespace EcoAssistant.Domain.Entities;


public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }

    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

}
