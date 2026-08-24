using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Reports;
using WMS.Application.Services.Reports;

namespace WMS.API.Controllers;

/// <summary>Reporting endpoints</summary>
[Route("api/v1/reports")]
public class ReportsController : BaseController
{
    private readonly IReportService _service;
    public ReportsController(IReportService service) => _service = service;

    [HttpGet("stock-balance")]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockBalanceReportDto>>>> StockBalance(
        [FromQuery] ReportFilterDto filter, CancellationToken ct)
        => OkResponse(await _service.GetStockBalanceReportAsync(filter, ct));

    [HttpGet("stock-movement")]
    public async Task<ActionResult<ApiResponse<IEnumerable<StockMovementReportDto>>>> StockMovement(
        [FromQuery] ReportFilterDto filter, CancellationToken ct)
        => OkResponse(await _service.GetStockMovementReportAsync(filter, ct));

    [HttpGet("grn")]
    public async Task<ActionResult<ApiResponse<IEnumerable<GRNReportDto>>>> GRN(
        [FromQuery] ReportFilterDto filter, CancellationToken ct)
        => OkResponse(await _service.GetGRNReportAsync(filter, ct));

    [HttpGet("shipments")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ShipmentReportDto>>>> Shipments(
        [FromQuery] ReportFilterDto filter, CancellationToken ct)
        => OkResponse(await _service.GetShipmentReportAsync(filter, ct));

    [HttpGet("cycle-count-variance")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CycleCountVarianceReportDto>>>> CycleCountVariance(
        [FromQuery] ReportFilterDto filter, CancellationToken ct)
        => OkResponse(await _service.GetCycleCountVarianceReportAsync(filter, ct));
}
