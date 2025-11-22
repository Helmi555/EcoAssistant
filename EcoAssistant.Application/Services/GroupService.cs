using EcoAssistant.Application.DTOs;
using EcoAssistant.Application.Interfaces;
using EcoAssistant.Domain.Entities;

namespace EcoAssistant.Application.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _repo;
    private readonly IIndustryCategoryRepository _industryCategoryRepository;
    private readonly IDeviceRepository _deviceRepository;

    public GroupService(IGroupRepository repo,IIndustryCategoryRepository industryCategoryRepository, IDeviceRepository deviceRepository   )
    {
        _repo = repo;
        _industryCategoryRepository = industryCategoryRepository;
        _deviceRepository = deviceRepository;
    }

    public async Task<Group?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _repo.GetByIdAsync(id, ct);
    }

    public async Task<List<Group>> GetAllAsync(CancellationToken ct = default)
    {
        return await _repo.GetAllAsync(ct);
    }

   public async Task<GroupDto> AddAsync(string name, string? description, Guid? industryCategoryId, CancellationToken ct = default)
    {
    Console.WriteLine($"Debug: Group Name = {name}, IndustryCategoryId = {industryCategoryId}");
    if (industryCategoryId != null)
        {
         Console.WriteLine($"Debug: IndustryCategoryId.Value = {industryCategoryId.Value}");

        var exists = await _industryCategoryRepository.ExistsAsync(industryCategoryId.Value, ct);
        if (!exists)
            throw new ArgumentException("Industry category not found", nameof(industryCategoryId));
    }

    var group = new Group
    {
        Id = Guid.NewGuid(),
        Name = name,
        Description = description,
        IndustryCategoryId = industryCategoryId,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow
    };

        group=await _repo.AddAsync(group, ct);
        return GroupDto.FromEntity(group);
    }


    public async Task UpdateAsync(Guid id, string name, string? description, Guid? industryCategoryId, CancellationToken ct = default)
    {
        var group = await _repo.GetByIdAsync(id, ct) ?? throw new InvalidOperationException("Group not found");

        if (industryCategoryId != null)
        {
            var exists = await _industryCategoryRepository.ExistsAsync(industryCategoryId.Value, ct);
            if (!exists)
                throw new ArgumentException("Industry category not found", nameof(industryCategoryId));
        }
        group.Name = name;
        group.Description = description;
        group.IndustryCategoryId = industryCategoryId;
        group.UpdatedAt = DateTimeOffset.UtcNow;

        await _repo.UpdateAsync(group, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _repo.DeleteAsync(id, ct);
    }

    public async Task AddDeviceToGroupAsync(Guid GroupId, int DeviceId, string DeviceName, CancellationToken ct = default)
    {
        var group = await _repo.GetByIdAsync(GroupId, ct) ?? throw new InvalidOperationException("Group not found");
        var device = await _deviceRepository.GetByIdAsync(DeviceId, ct) ?? throw new InvalidOperationException("Device not found");
        if(group.Devices.Any(d => d.Id == DeviceId))
        {
            throw new InvalidOperationException("Device already in group");
        }
        group.Devices.Add(device);
        device.DeviceName = DeviceName;
        device.GroupId = group.Id;
        device.Group = group;

        await _repo.UpdateAsync(group);
        await _deviceRepository.UpdateAsync(device);
    }

    public async Task RemoveDeviceFromGroupAsync(Guid GroupId, int DeviceId, CancellationToken ct = default)
    {
    var group = await _repo.GetByIdAsync(GroupId, ct) ?? throw new InvalidOperationException("Group not found");
    var device = await _deviceRepository.GetByIdAsync(DeviceId, ct) ?? throw new InvalidOperationException("Device not found");

var trackedDevice = group.Devices.FirstOrDefault(d => d.Id == device.Id);
if (trackedDevice != null)
    group.Devices.Remove(trackedDevice);
    device.GroupId = null;
    device.Group = null;

    await _repo.UpdateAsync(group);
    await _deviceRepository.UpdateAsync(device);
}
}