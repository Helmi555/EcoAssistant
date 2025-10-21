using EcoAssistant.Application.Interfaces;
using EcoAssistant.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EcoAssistant.API.Controllers;

[ApiController]
[Route("api/groups")]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Group>> GetById(Guid id, CancellationToken ct)
    {
        var group = await _groupService.GetByIdAsync(id, ct);
        return group is null ? NotFound() : Ok(group);
    }

    [HttpGet]
    public async Task<ActionResult<List<Group>>> GetAll(CancellationToken ct)
    {
        var groups = await _groupService.GetAllAsync(ct);
        return Ok(groups);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] GroupDto dto, CancellationToken ct)
    { try
    {
        await _groupService.AddAsync(dto.Name, dto.Description, dto.IndustryCategoryId, ct);
        return CreatedAtAction(nameof(GetById), null);
    }
    catch (ArgumentException ex) when (ex.ParamName == "industryCategoryId")
    {
        return BadRequest(new { error = "Industry category not found" });
    }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] GroupDto dto, CancellationToken ct)
    {
        await _groupService.UpdateAsync(id, dto.Name, dto.Description, dto.IndustryCategoryId, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _groupService.DeleteAsync(id, ct);
        return NoContent();
    }
}
