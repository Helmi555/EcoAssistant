using EcoAssistant.Domain.Entities;

public interface IUserGroupService
{
    Task<UserGroup?> GetByIdsAsync(Guid userId, Guid groupId, CancellationToken ct = default);
    Task<List<UserGroup>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<List<UserGroup>> GetByGroupIdAsync(Guid groupId, CancellationToken ct = default);
    Task<UserGroup> AddAsync(Guid userId, Guid groupId, GroupRole role, GroupStatus status, CancellationToken ct = default);
    Task UpdateAsync(Guid userId, Guid groupId, GroupRole role, GroupStatus status, CancellationToken ct = default);
    Task DeleteAsync(Guid userId, Guid groupId, CancellationToken ct = default);
}