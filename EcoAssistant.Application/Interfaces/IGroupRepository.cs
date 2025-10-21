using EcoAssistant.Domain.Entities;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Group>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Group group, CancellationToken ct = default);
    Task UpdateAsync(Group group, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}