using EcoAssistant.Application.Interfaces;
using EcoAssistant.Domain.Entities;
using EcoAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoAssistant.Infrastructure.Repositories;

public class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public EfUserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user;
    }

    public async Task<List<User>> ListAsync(int limit = 50, int offset = 0, CancellationToken ct = default) =>
        await _db.Users.AsNoTracking().OrderBy(u => u.CreatedAt).Skip(offset).Take(limit).ToListAsync(ct);

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        user.UpdatedAt = DateTimeOffset.UtcNow;
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }
}
