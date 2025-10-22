using EcoAssistant.Application;
using EcoAssistant.Domain.Entities;
using EcoAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoAssistant.Infrastructure.Repositories;

public class EfIndustryCategoryRepository : IIndustryCategoryRepository
{
    private readonly AppDbContext _db;

    public EfIndustryCategoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IndustryCategory?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.IndustryCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<List<IndustryCategory>> GetAllAsync(CancellationToken ct = default) =>
        await _db.IndustryCategories.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(IndustryCategory category, CancellationToken ct = default)
    {
        _db.IndustryCategories.Add(category);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(IndustryCategory category, CancellationToken ct = default)
    {
        category.UpdatedAt = DateTimeOffset.UtcNow;
        _db.IndustryCategories.Update(category);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _db.IndustryCategories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category != null)
        {
            _db.IndustryCategories.Remove(category);
            await _db.SaveChangesAsync(ct);
        }
    }

public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
    await _db.IndustryCategories.AnyAsync(ic => ic.Id == id, ct);
}