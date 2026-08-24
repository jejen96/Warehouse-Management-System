using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Inbound;
using WMS.Application.Services.Inbound;

namespace WMS.API.Controllers;

/// <summary>Goods Receipt Note management</summary>
[Route("api/v1/grn")]
public class GRNController : BaseController
{
    private readonly IGRNService _service;
    public GRNController(IGRNService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<GRNDto>>>> GetAll([FromQuery] PaginationParams pagination, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GRNDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager,Operator")]
    public async Task<ActionResult<ApiResponse<GRNDto>>> Create([FromBody] CreateGRNDto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));

    [HttpPost("{id:guid}/complete")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<ActionResult<ApiResponse<GRNDto>>> Complete(Guid id, CancellationToken ct)
        => OkResponse(await _service.CompleteAsync(id, CurrentUser, ct));
}
