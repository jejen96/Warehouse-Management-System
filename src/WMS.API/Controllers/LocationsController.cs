using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.MasterData;
using WMS.Application.Services.MasterData;

namespace WMS.API.Controllers;

/// <summary>Location (Bin) management</summary>
public class LocationsController : BaseController
{
    private readonly ILocationService _service;
    public LocationsController(ILocationService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<LocationDto>>>> GetAll(
        [FromQuery] PaginationParams pagination, [FromQuery] Guid? warehouseId, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, warehouseId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<LocationDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<ActionResult<ApiResponse<LocationDto>>> Create([FromBody] CreateLocationDto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<ActionResult<ApiResponse<LocationDto>>> Update(Guid id, [FromBody] UpdateLocationDto dto, CancellationToken ct)
        => OkResponse(await _service.UpdateAsync(id, dto, CurrentUser, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CurrentUser, ct);
        return OkResponse<object>(null!, "Location deleted.");
    }
}
