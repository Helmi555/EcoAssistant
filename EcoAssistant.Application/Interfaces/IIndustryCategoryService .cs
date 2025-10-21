using EcoAssistant.Domain.Entities;

public interface IIndustryCategoryService
{
    Task<IndustryCategory?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<IndustryCategory>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(string name, string? description, CancellationToken ct = default);
    Task UpdateAsync(Guid id, string name, string? description, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}