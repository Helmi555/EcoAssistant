using EcoAssistant.Domain.Entities;
using EcoAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoAssistant.Infrastructure.Repositories;

public class EfGroupRepository : IGroupRepository
{
    private readonly AppDbContext _db;

    public EfGroupRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Group?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Groups.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<List<Group>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Groups.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Group group, CancellationToken ct = default)
    {
        _db.Groups.Add(group);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Group group, CancellationToken ct = default)
    {
        group.UpdatedAt = DateTimeOffset.UtcNow;
        _db.Groups.Update(group);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var group = await _db.Groups.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (group != null)
        {
            _db.Groups.Remove(group);
            await _db.SaveChangesAsync(ct);
        }
    }
}