using Microsoft.AspNetCore.Mvc;
using EcoAssistant.Application.Services;
using EcoAssistant.API.Dtos;

namespace EcoAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _svc;
    public UsersController(UserService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req, CancellationToken ct)
    {
        try
        {
            var dto = await _svc.CreateAsync(req.Name, req.LastName, req.Username, req.Password, req.DateOfBirth, req.Address, ct);
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }
        catch (InvalidOperationException ex) when (ex.Message == "username_taken")
        {
            return Conflict(new { error = "username_taken" });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var dto = await _svc.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int limit = 50, [FromQuery] int offset = 0, CancellationToken ct = default) =>
        Ok(await _svc.ListAsync(limit, offset, ct));
}
