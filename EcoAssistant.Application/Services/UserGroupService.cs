using EcoAssistant.Application.Interfaces;
using EcoAssistant.Domain.Entities;

namespace EcoAssistant.Application.Services;

public class UserGroupService : IUserGroupService
{
    private readonly IUserGroupRepository _repo;

    public UserGroupService(IUserGroupRepository repo)
    {
        _repo = repo;
    }

    public async Task<UserGroup?> GetByIdsAsync(Guid userId, Guid groupId, CancellationToken ct = default)
    {
        return await _repo.GetByIdsAsync(userId, groupId, ct);
    }

    public async Task<List<UserGroup>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _repo.GetByUserIdAsync(userId, ct);
    }

    public async Task<List<UserGroup>> GetByGroupIdAsync(Guid groupId, CancellationToken ct = default)
    {
        return await _repo.GetByGroupIdAsync(groupId, ct);
    }

    public async Task AddAsync(Guid userId, Guid groupId, GroupRole role, GroupStatus status, CancellationToken ct = default)
    {
        var userGroup = new UserGroup
        {
            UserId = userId,
            GroupId = groupId,
            Role = role,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(userGroup, ct);
    }

    public async Task UpdateAsync(Guid userId, Guid groupId, GroupRole role, GroupStatus status, CancellationToken ct = default)
    {
        var userGroup = await _repo.GetByIdsAsync(userId, groupId, ct)
            ?? throw new InvalidOperationException("UserGroup not found");

        userGroup.Role = role;
        userGroup.Status = status;
        userGroup.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(userGroup, ct);
    }

    public async Task DeleteAsync(Guid userId, Guid groupId, CancellationToken ct = default)
    {
        await _repo.DeleteAsync(userId, groupId, ct);
    }
}