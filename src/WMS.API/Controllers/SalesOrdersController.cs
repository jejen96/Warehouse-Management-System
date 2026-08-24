using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Outbound;
using WMS.Application.Services.Outbound;
using WMS.Domain.Enums;

namespace WMS.API.Controllers;

/// <summary>Sales Order management</summary>
public class SalesOrdersController : BaseController
{
    private readonly ISalesOrderService _service;
    public SalesOrdersController(ISalesOrderService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SalesOrderDto>>>> GetAll(
        [FromQuery] PaginationParams pagination, [FromQuery] SOStatus? status, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, status, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<SalesOrderDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager,Operator")]
    public async Task<ActionResult<ApiResponse<SalesOrderDto>>> Create([FromBody] CreateSODto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<ActionResult<ApiResponse<SalesOrderDto>>> UpdateStatus(Guid id, [FromBody] UpdateSOStatusDto dto, CancellationToken ct)
        => OkResponse(await _service.UpdateStatusAsync(id, dto, CurrentUser, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CurrentUser, ct);
        return OkResponse<object>(null!, "Sales Order deleted.");
    }
}
