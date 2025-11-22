using EcoAssistant.Domain.Entities;
using EcoAssistant.Application.DTOs;

public interface IGroupService
{
    Task<Group?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Group>> GetAllAsync(CancellationToken ct = default);
    Task<GroupDto> AddAsync(string name, string? description, Guid? industryCategoryId, CancellationToken ct = default);
    Task UpdateAsync(Guid id, string name, string? description, Guid? industryCategoryId, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    Task AddDeviceToGroupAsync(Guid GroupId, int DeviceId, string DeviceName, CancellationToken ct = default);
    Task RemoveDeviceFromGroupAsync(Guid GroupId, int DeviceId, CancellationToken ct = default);
}