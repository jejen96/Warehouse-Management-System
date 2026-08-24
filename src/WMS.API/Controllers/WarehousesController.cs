using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.MasterData;
using WMS.Application.Services.MasterData;

namespace WMS.API.Controllers;

/// <summary>Warehouse master data management</summary>
public class WarehousesController : BaseController
{
    private readonly IWarehouseService _warehouseService;
    private readonly ILocationService _locationService;

    public WarehousesController(IWarehouseService warehouseService, ILocationService locationService)
    {
        _warehouseService = warehouseService; _locationService = locationService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<WarehouseDto>>>> GetAll([FromQuery] PaginationParams pagination, CancellationToken ct)
        => OkResponse(await _warehouseService.GetAllAsync(pagination, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<WarehouseDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _warehouseService.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<WarehouseDto>>> Create([FromBody] CreateWarehouseDto dto, CancellationToken ct)
        => CreatedResponse(await _warehouseService.CreateAsync(dto, CurrentUser, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<WarehouseDto>>> Update(Guid id, [FromBody] UpdateWarehouseDto dto, CancellationToken ct)
        => OkResponse(await _warehouseService.UpdateAsync(id, dto, CurrentUser, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _warehouseService.DeleteAsync(id, CurrentUser, ct);
        return OkResponse<object>(null!, "Warehouse deleted.");
    }
}
