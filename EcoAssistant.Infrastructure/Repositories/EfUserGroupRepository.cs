using EcoAssistant.Application.Interfaces;
using EcoAssistant.Domain.Entities;
using EcoAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoAssistant.Infrastructure.Repositories;

public class EfUserGroupRepository : IUserGroupRepository
{
    private readonly AppDbContext _db;

    public EfUserGroupRepository(AppDbContext db) => _db = db;

    public async Task<UserGroup?> GetByIdsAsync(Guid userId, Guid groupId, CancellationToken ct = default) =>
        await _db.UserGroups
                 .AsNoTracking()
                 .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId, ct);

    public async Task<List<UserGroup>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _db.UserGroups
                 .AsNoTracking()
                 .Where(ug => ug.UserId == userId)
                 .ToListAsync(ct);

    public async Task<List<UserGroup>> GetByGroupIdAsync(Guid groupId, CancellationToken ct = default) =>
        await _db.UserGroups
                 .AsNoTracking()
                 .Where(ug => ug.GroupId == groupId)
                 .ToListAsync(ct);

    public async Task<UserGroup> AddAsync(UserGroup userGroup, CancellationToken ct = default)
    {
        _db.UserGroups.Add(userGroup);
        await _db.SaveChangesAsync(ct);
        return userGroup;
    }

    public async Task UpdateAsync(UserGroup userGroup, CancellationToken ct = default)
    {
        userGroup.UpdatedAt = DateTime.UtcNow;
        _db.UserGroups.Update(userGroup);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid userId, Guid groupId, CancellationToken ct = default)
    {
        var userGroup = await _db.UserGroups
                                 .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId, ct);
        if (userGroup != null)
        {
            _db.UserGroups.Remove(userGroup);
            await _db.SaveChangesAsync(ct);
        }
    }
}