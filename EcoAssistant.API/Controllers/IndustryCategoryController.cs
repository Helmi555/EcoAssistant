using EcoAssistant.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EcoAssistant.API.Controllers;

[ApiController]
[Route("api/industry-categories")]
public class IndustryCategoryController : ControllerBase
{
    private readonly IIndustryCategoryService _service;

    public IndustryCategoryController(IIndustryCategoryService service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var category = await _service.GetByIdAsync(id, ct);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var categories = await _service.GetAllAsync(ct);
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] IndustryCategory category, CancellationToken ct)
    {
        await _service.AddAsync(category.Name, category.Description, ct);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] IndustryCategory category, CancellationToken ct)
    {
        await _service.UpdateAsync(id, category.Name, category.Description, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}