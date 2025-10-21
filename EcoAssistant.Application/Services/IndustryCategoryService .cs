using EcoAssistant.Domain.Entities;

namespace EcoAssistant.Application.Services;

public class IndustryCategoryService : IIndustryCategoryService
{
    private readonly IIndustryCategoryRepository _repo;

    public IndustryCategoryService(IIndustryCategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<IndustryCategory?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _repo.GetByIdAsync(id, ct);
    }

    public async Task<List<IndustryCategory>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repo.GetAllAsync(ct);
    }

    public async Task AddAsync(string name, string? description, CancellationToken ct = default)
    {
        var category = new IndustryCategory
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _repo.AddAsync(category, ct);
    }

    public async Task UpdateAsync(Guid id, string name, string? description, CancellationToken ct = default)
    {
        var category = await _repo.GetByIdAsync(id, ct) ?? throw new InvalidOperationException("IndustryCategory not found");

        category.Name = name;
        category.Description = description;
        category.UpdatedAt = DateTimeOffset.UtcNow;

        await _repo.UpdateAsync(category, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _repo.DeleteAsync(id, ct);
    }
}