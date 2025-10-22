using EcoAssistant.Application.DTOs;
using EcoAssistant.Application.Interfaces;
using EcoAssistant.Domain.Entities;

namespace EcoAssistant.Application.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _repo;
    private readonly IIndustryCategoryRepository _industryCategoryRepository;

    public GroupService(IGroupRepository repo,IIndustryCategoryRepository industryCategoryRepository)
    {
        _repo = repo;
        _industryCategoryRepository = industryCategoryRepository;
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
}