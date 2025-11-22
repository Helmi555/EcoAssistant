namespace EcoAssistant.Domain.Entities;


public class Group
{
    public Guid Id { get; set; } =  Guid.NewGuid();
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    public Guid? IndustryCategoryId { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    //Les relations lenna 

    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<Device> Devices { get; set; } = new List<Device>();
    public IndustryCategory? IndustryCategory { get; set; }

}


