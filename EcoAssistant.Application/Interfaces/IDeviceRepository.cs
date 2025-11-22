using EcoAssistant.Domain.Entities;


namespace EcoAssistant.Application.Interfaces;

public interface IDeviceRepository
{

    Task<Device?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Device>> GetAllAsync(CancellationToken ct = default);
    Task<Device> AddAsync(Device device, CancellationToken ct = default);
    Task UpdateAsync(Device device, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);

    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

}

