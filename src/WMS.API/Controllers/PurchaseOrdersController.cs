using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Inbound;
using WMS.Application.Services.Inbound;
using WMS.Domain.Enums;

namespace WMS.API.Controllers;

/// <summary>Purchase Order management</summary>
public class PurchaseOrdersController : BaseController
{
    private readonly IPurchaseOrderService _service;
    public PurchaseOrdersController(IPurchaseOrderService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PurchaseOrderDto>>>> GetAll(
        [FromQuery] PaginationParams pagination, [FromQuery] POStatus? status, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, status, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager,Operator")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderDto>>> Create([FromBody] CreatePODto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<ActionResult<ApiResponse<PurchaseOrderDto>>> UpdateStatus(Guid id, [FromBody] UpdatePOStatusDto dto, CancellationToken ct)
        => OkResponse(await _service.UpdateStatusAsync(id, dto, CurrentUser, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CurrentUser, ct);
        return OkResponse<object>(null!, "Purchase Order deleted.");
    }
}
