using EcoAssistant.Application.Interfaces;
using EcoAssistant.Domain.Entities;

namespace EcoAssistant.Application.Services;

public class UserGroupService : IUserGroupService
{
    private readonly IUserGroupRepository _repo;
    private readonly IUserRepository _userRepo;

    private readonly IGroupRepository _groupRepo;

    public UserGroupService(IUserGroupRepository repo,IUserRepository userGroupRepository,IGroupRepository groupRepository)
    {
        _repo = repo;
        _userRepo = userGroupRepository;
        _groupRepo = groupRepository;

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

    public async Task<UserGroup> AddAsync(Guid userId, Guid groupId, GroupRole role, GroupStatus status, CancellationToken ct = default)
    {
         // Check if UserId exists
    if (!await _userRepo.ExistsAsync(userId, ct))
        throw new ArgumentException("User not found");

        // Check if GroupId exists  
        if (!await _groupRepo.ExistsAsync(groupId, ct))
            throw new ArgumentException("Group not found");


        if (await _repo.GetByIdsAsync(userId, groupId, ct)!=null)
            throw new ArgumentException("UserGroup relationship already exists");
        
        var userGroup = new UserGroup
        {
            UserId = userId,
            GroupId = groupId,
            Role = role,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        userGroup = await _repo.AddAsync(userGroup, ct);
        return userGroup;
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