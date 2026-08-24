using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Outbound;
using WMS.Application.Services.Outbound;

namespace WMS.API.Controllers;

/// <summary>Picking list management</summary>
[Route("api/v1/picking-lists")]
public class PickingListsController : BaseController
{
    private readonly IPickingService _service;
    public PickingListsController(IPickingService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PickingListDto>>>> GetAll([FromQuery] PaginationParams pagination, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PickingListDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager,Operator")]
    public async Task<ActionResult<ApiResponse<PickingListDto>>> Create([FromBody] CreatePickingListDto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));

    [HttpPost("{id:guid}/complete")]
    [Authorize(Roles = "Admin,WarehouseManager,Operator")]
    public async Task<ActionResult<ApiResponse<PickingListDto>>> Complete(Guid id, [FromBody] List<UpdatePickedQtyDto> updates, CancellationToken ct)
        => OkResponse(await _service.CompletePickingAsync(id, updates, CurrentUser, ct));
}

/// <summary>Packing management</summary>
[Route("api/v1/packings")]
public class PackingsController : BaseController
{
    private readonly IPackingService _service;
    public PackingsController(IPackingService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PackingDto>>>> GetAll([FromQuery] PaginationParams pagination, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<PackingDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager,Operator")]
    public async Task<ActionResult<ApiResponse<PackingDto>>> Create([FromBody] CreatePackingDto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));
}

/// <summary>Shipment management</summary>
[Route("api/v1/shipments")]
public class ShipmentsController : BaseController
{
    private readonly IShipmentService _service;
    public ShipmentsController(IShipmentService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ShipmentDto>>>> GetAll([FromQuery] PaginationParams pagination, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ShipmentDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager,Operator")]
    public async Task<ActionResult<ApiResponse<ShipmentDto>>> Create([FromBody] CreateShipmentDto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));
}
