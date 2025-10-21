using EcoAssistant.Domain.Entities;

public interface IGroupService
{
    Task<Group?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Group>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(string name, string? description, Guid? industryCategoryId, CancellationToken ct = default);
    Task UpdateAsync(Guid id, string name, string? description, Guid? industryCategoryId, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}