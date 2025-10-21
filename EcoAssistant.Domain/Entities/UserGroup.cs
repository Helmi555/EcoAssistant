namespace EcoAssistant.Domain.Entities;

public class UserGroup
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }

    public GroupRole Role { get; set; } = GroupRole.Viewer;
    public GroupStatus Status { get; set; } = GroupStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Group Group { get; set; } = null!;
}
