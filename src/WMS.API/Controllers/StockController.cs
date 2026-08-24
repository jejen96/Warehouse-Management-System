using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Inventory;
using WMS.Application.Services.Inventory;

namespace WMS.API.Controllers;

/// <summary>Stock balance and ledger queries</summary>
[Route("api/v1/stock")]
public class StockController : BaseController
{
    private readonly IStockQueryService _queryService;
    public StockController(IStockQueryService queryService) => _queryService = queryService;

    [HttpGet("balances")]
    public async Task<ActionResult<ApiResponse<PagedResult<StockBalanceDto>>>> GetBalances(
        [FromQuery] PaginationParams pagination, [FromQuery] Guid? itemId,
        [FromQuery] Guid? locationId, [FromQuery] Guid? warehouseId, CancellationToken ct)
        => OkResponse(await _queryService.GetStockBalancesAsync(pagination, itemId, locationId, warehouseId, ct));

    [HttpGet("ledger")]
    public async Task<ActionResult<ApiResponse<PagedResult<StockLedgerDto>>>> GetLedger(
        [FromQuery] PaginationParams pagination, [FromQuery] Guid? itemId,
        [FromQuery] Guid? locationId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
        => OkResponse(await _queryService.GetStockLedgerAsync(pagination, itemId, locationId, from, to, ct));
}

/// <summary>Stock adjustment management</summary>
[Route("api/v1/stock-adjustments")]
public class StockAdjustmentsController : BaseController
{
    private readonly IStockAdjustmentService _service;
    public StockAdjustmentsController(IStockAdjustmentService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<StockAdjustmentDto>>>> GetAll([FromQuery] PaginationParams pagination, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<StockAdjustmentDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager,Operator")]
    public async Task<ActionResult<ApiResponse<StockAdjustmentDto>>> Create([FromBody] CreateStockAdjustmentDto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<ActionResult<ApiResponse<StockAdjustmentDto>>> Approve(Guid id, CancellationToken ct)
        => OkResponse(await _service.ApproveAsync(id, CurrentUser, ct));
}

/// <summary>Stock transfer management</summary>
[Route("api/v1/stock-transfers")]
public class StockTransfersController : BaseController
{
    private readonly IStockTransferService _service;
    public StockTransfersController(IStockTransferService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<StockTransferDto>>>> GetAll([FromQuery] PaginationParams pagination, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<StockTransferDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager,Operator")]
    public async Task<ActionResult<ApiResponse<StockTransferDto>>> Create([FromBody] CreateStockTransferDto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));
}

/// <summary>Cycle count management</summary>
[Route("api/v1/cycle-counts")]
public class CycleCountsController : BaseController
{
    private readonly ICycleCountService _service;
    public CycleCountsController(ICycleCountService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CycleCountDto>>>> GetAll([FromQuery] PaginationParams pagination, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CycleCountDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager,Operator")]
    public async Task<ActionResult<ApiResponse<CycleCountDto>>> Create([FromBody] CreateCycleCountDto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));

    [HttpPost("{id:guid}/adjust")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<ActionResult<ApiResponse<CycleCountDto>>> Adjust(Guid id, CancellationToken ct)
        => OkResponse(await _service.AdjustAsync(id, CurrentUser, ct));
}
