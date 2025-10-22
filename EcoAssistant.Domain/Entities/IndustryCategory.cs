namespace EcoAssistant.Domain.Entities;

public class IndustryCategory
{

    public Guid Id { get; set; } =  Guid.NewGuid();
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    
}