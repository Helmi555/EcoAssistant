using EcoAssistant.Application.Interfaces;
using EcoAssistant.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using EcoAssistant.API.Dtos;

namespace EcoAssistant.API.Controllers;

[ApiController]
[Route("api/user-groups")]
public class UserGroupController : ControllerBase
{
    private readonly IUserGroupService _service;

    public UserGroupController(IUserGroupService service)
    {
        _service = service;
    }

    [HttpGet("{userId:Guid}/{groupId:Guid}")]
    public async Task<IActionResult> GetByIds(Guid userId, Guid groupId, CancellationToken ct)
    {
        var userGroup = await _service.GetByIdsAsync(userId, groupId, ct);
        return userGroup is null ? NotFound() : Ok(userGroup);
    }

    [HttpGet("user/{userId:Guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken ct)
    {
        var userGroups = await _service.GetByUserIdAsync(userId, ct);
        return Ok(userGroups);
    }

    [HttpGet("group/{groupId:Guid}")]
    public async Task<IActionResult> GetByGroupId(Guid groupId, CancellationToken ct)
    {
        var userGroups = await _service.GetByGroupIdAsync(groupId, ct);
        return Ok(userGroups);
    }

   [HttpPost]
public async Task<IActionResult> Add([FromBody] UserGroupRequestDto userGroup, CancellationToken ct)
{
    try
    {
        await _service.AddAsync(userGroup.UserId, userGroup.GroupId, userGroup.Role, userGroup.Status, ct);
        return Ok(userGroup);
    }
    catch (Exception ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}

    [HttpPut("{userId:Guid}/{groupId:Guid}")]
    public async Task<IActionResult> Update(Guid userId, Guid groupId, [FromBody] UserGroup userGroup, CancellationToken ct)
    {
        await _service.UpdateAsync(userId, groupId, userGroup.Role, userGroup.Status, ct);
        return NoContent();
    }

    [HttpDelete("{userId:Guid}/{groupId:Guid}")]
    public async Task<IActionResult> Delete(Guid userId, Guid groupId, CancellationToken ct)
    {
        await _service.DeleteAsync(userId, groupId, ct);
        return NoContent();
    }
}