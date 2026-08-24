using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.MasterData;
using WMS.Application.Services.MasterData;

namespace WMS.API.Controllers;

/// <summary>Item (Product) master data management</summary>
public class ItemsController : BaseController
{
    private readonly IItemService _service;
    public ItemsController(IItemService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ItemDto>>>> GetAll(
        [FromQuery] PaginationParams pagination, [FromQuery] string? search, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, search, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ItemDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<ActionResult<ApiResponse<ItemDto>>> Create([FromBody] CreateItemDto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<ActionResult<ApiResponse<ItemDto>>> Update(Guid id, [FromBody] UpdateItemDto dto, CancellationToken ct)
        => OkResponse(await _service.UpdateAsync(id, dto, CurrentUser, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CurrentUser, ct);
        return OkResponse<object>(null!, "Item deleted successfully.");
    }
}
