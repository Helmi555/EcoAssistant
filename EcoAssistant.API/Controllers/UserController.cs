using Microsoft.AspNetCore.Mvc;
using EcoAssistant.Application.Services;
using EcoAssistant.API.Dtos;
using EcoAssistant.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace EcoAssistant.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _svc;
    public UsersController(IUserService svc) => _svc = svc;

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
    [Authorize]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var dto = await _svc.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int limit = 50, [FromQuery] int offset = 0, CancellationToken ct = default) =>
        Ok(await _svc.ListAsync(limit, offset, ct));


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        try
        {
            var token = await _svc.LoginAsync(request.Username, request.Password);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });

        }
    }

}
