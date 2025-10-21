using EcoAssistant.Domain.Entities;

public interface IIndustryCategoryRepository
{
    Task<IndustryCategory?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<IndustryCategory>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(IndustryCategory category, CancellationToken ct = default);
    Task UpdateAsync(IndustryCategory category, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}