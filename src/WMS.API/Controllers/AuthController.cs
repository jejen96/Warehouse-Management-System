using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Security;
using WMS.Application.Services.Security;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Login and receive JWT token</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(dto, ct);
        return Ok(ApiResponse<LoginResponseDto>.Ok(result, "Login successful."));
    }

    /// <summary>Register a new user (Admin only)</summary>
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] CreateUserDto dto, CancellationToken ct)
    {
        var username = User.Identity?.Name ?? "system";
        var result = await _authService.RegisterAsync(dto, username, ct);
        return StatusCode(201, ApiResponse<UserDto>.Ok(result, "User registered successfully."));
    }
}
