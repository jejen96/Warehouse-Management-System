using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Security;
using WMS.Application.Services.Security;
using WMS.API.Extensions;

namespace WMS.API.Controllers;

/// <summary>User management</summary>
public class UsersController : BaseController
{
    private readonly IUserService _service;
    public UsersController(IUserService service) => _service = service;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetAll([FromQuery] PaginationParams pagination, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, ct));

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(Guid id, [FromBody] UpdateUserDto dto, CancellationToken ct)
        => OkResponse(await _service.UpdateAsync(id, dto, CurrentUser, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CurrentUser, ct);
        return OkResponse<object>(null!, "User deleted.");
    }

    [HttpPost("{id:guid}/change-password")]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto, CancellationToken ct)
    {
        // Users can only change their own password unless Admin
        var currentUserId = User.GetUserId();
        if (currentUserId != id && !User.IsInRole("Admin"))
            return Forbid();

        await _service.ChangePasswordAsync(id, dto, ct);
        return OkResponse<object>(null!, "Password changed successfully.");
    }
}
