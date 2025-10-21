using EcoAssistant.Domain.Entities;

namespace EcoAssistant.Application.Interfaces;


public interface IUserGroupRepository
{
    Task<UserGroup?> GetByIdsAsync(Guid userId, Guid groupId, CancellationToken ct = default);
    Task<List<UserGroup>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<List<UserGroup>> GetByGroupIdAsync(Guid groupId, CancellationToken ct = default);
    Task AddAsync(UserGroup userGroup, CancellationToken ct = default);
    Task UpdateAsync(UserGroup userGroup, CancellationToken ct = default);
    Task DeleteAsync(Guid userId, Guid groupId, CancellationToken ct = default);

}