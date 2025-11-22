using EcoAssistant.Domain.Entities;
using EcoAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EcoAssistant.Application.Interfaces;

namespace EcoAssistant.Infrastructure.Repositories;

public class EfDeviceRepository : IDeviceRepository
{
    private readonly AppDbContext _db;

    public EfDeviceRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<Device?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Devices.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<List<Device>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Devices.AsNoTracking().ToListAsync(ct);

    public async Task<Device> AddAsync(Device device, CancellationToken ct = default)
    {
        _db.Devices.Add(device);
        await _db.SaveChangesAsync(ct);
        return device;
    }

    public async Task UpdateAsync(Device device, CancellationToken ct = default)
    {
        device.UpdatedAt = DateTimeOffset.UtcNow;
        _db.Devices.Update(device);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var device = await _db.Devices.FirstOrDefaultAsync(d => d.Id == id, ct);
        if (device != null)
        {
            _db.Devices.Remove(device);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return await _db.Devices.AnyAsync(d => d.Id == id, ct);
    }
}