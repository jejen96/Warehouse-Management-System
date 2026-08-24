using Microsoft.EntityFrameworkCore;
using WMS.Application.DTOs.Reports;
using WMS.Domain.Entities.Inbound;
using WMS.Domain.Entities.Inventory;
using WMS.Domain.Entities.Outbound;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Reports;

public interface IReportService
{
    Task<IEnumerable<StockBalanceReportDto>> GetStockBalanceReportAsync(ReportFilterDto filter, CancellationToken ct = default);
    Task<IEnumerable<StockMovementReportDto>> GetStockMovementReportAsync(ReportFilterDto filter, CancellationToken ct = default);
    Task<IEnumerable<GRNReportDto>> GetGRNReportAsync(ReportFilterDto filter, CancellationToken ct = default);
    Task<IEnumerable<ShipmentReportDto>> GetShipmentReportAsync(ReportFilterDto filter, CancellationToken ct = default);
    Task<IEnumerable<CycleCountVarianceReportDto>> GetCycleCountVarianceReportAsync(ReportFilterDto filter, CancellationToken ct = default);
}

public class ReportService : IReportService
{
    private readonly IRepository<StockBalance> _stockBalanceRepo;
    private readonly IRepository<StockLedger> _ledgerRepo;
    private readonly IRepository<GoodsReceiptNote> _grnRepo;
    private readonly IRepository<Shipment> _shipmentRepo;
    private readonly IRepository<CycleCount> _cycleCountRepo;

    public ReportService(IRepository<StockBalance> stockBalanceRepo, IRepository<StockLedger> ledgerRepo,
        IRepository<GoodsReceiptNote> grnRepo, IRepository<Shipment> shipmentRepo, IRepository<CycleCount> cycleCountRepo)
    {
        _stockBalanceRepo = stockBalanceRepo; _ledgerRepo = ledgerRepo;
        _grnRepo = grnRepo; _shipmentRepo = shipmentRepo; _cycleCountRepo = cycleCountRepo;
    }

    public async Task<IEnumerable<StockBalanceReportDto>> GetStockBalanceReportAsync(ReportFilterDto filter, CancellationToken ct = default)
    {
        var query = _stockBalanceRepo.Query()
            .Include(x => x.Item).Include(x => x.Location).ThenInclude(l => l.Warehouse)
            .Where(x => !x.IsDeleted);

        if (filter.ItemId.HasValue) query = query.Where(x => x.ItemId == filter.ItemId.Value);
        if (filter.LocationId.HasValue) query = query.Where(x => x.LocationId == filter.LocationId.Value);
        if (filter.WarehouseId.HasValue) query = query.Where(x => x.Location.WarehouseId == filter.WarehouseId.Value);

        var result = await query.ToListAsync(ct);
        return result.Select(x => new StockBalanceReportDto(
            x.Item.ItemCode, x.Item.ItemName, x.Location.LocationCode,
            x.Location.Warehouse?.WarehouseName ?? "", x.AvailableQty, x.ReservedQty, x.AvailableQty + x.ReservedQty));
    }

    public async Task<IEnumerable<StockMovementReportDto>> GetStockMovementReportAsync(ReportFilterDto filter, CancellationToken ct = default)
    {
        var query = _ledgerRepo.Query()
            .Include(x => x.Item).Include(x => x.Location)
            .Where(x => !x.IsDeleted);

        if (filter.FromDate.HasValue) query = query.Where(x => x.CreatedAt >= filter.FromDate.Value);
        if (filter.ToDate.HasValue) query = query.Where(x => x.CreatedAt <= filter.ToDate.Value);
        if (filter.ItemId.HasValue) query = query.Where(x => x.ItemId == filter.ItemId.Value);

        var result = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
        return result.Select(x => new StockMovementReportDto(
            x.CreatedAt, x.Item.ItemCode, x.Item.ItemName, x.Location.LocationCode,
            x.MovementType.ToString(), x.ReferenceNumber, x.Quantity, x.CreatedBy));
    }

    public async Task<IEnumerable<GRNReportDto>> GetGRNReportAsync(ReportFilterDto filter, CancellationToken ct = default)
    {
        var query = _grnRepo.Query()
            .Include(x => x.PurchaseOrder).ThenInclude(p => p.Vendor)
            .Include(x => x.Details)
            .Where(x => !x.IsDeleted);

        if (filter.FromDate.HasValue) query = query.Where(x => x.GRNDate >= filter.FromDate.Value);
        if (filter.ToDate.HasValue) query = query.Where(x => x.GRNDate <= filter.ToDate.Value);

        var result = await query.OrderByDescending(x => x.GRNDate).ToListAsync(ct);
        return result.Select(x => new GRNReportDto(
            x.GRNNumber, x.GRNDate, x.PurchaseOrder.PONumber,
            x.PurchaseOrder.Vendor?.VendorName ?? "", x.ReceivedBy,
            x.Status.ToString(), x.Details.Count, x.Details.Sum(d => d.ReceivedQty)));
    }

    public async Task<IEnumerable<ShipmentReportDto>> GetShipmentReportAsync(ReportFilterDto filter, CancellationToken ct = default)
    {
        var query = _shipmentRepo.Query()
            .Include(x => x.Packing).ThenInclude(p => p.SalesOrder)
            .Where(x => !x.IsDeleted);

        if (filter.FromDate.HasValue) query = query.Where(x => x.ShippedDate >= filter.FromDate.Value);
        if (filter.ToDate.HasValue) query = query.Where(x => x.ShippedDate <= filter.ToDate.Value);

        var result = await query.OrderByDescending(x => x.ShippedDate).ToListAsync(ct);
        return result.Select(x => new ShipmentReportDto(
            x.ShipmentNumber, x.ShippedDate, x.Packing.SalesOrder.SONumber,
            x.Packing.SalesOrder.CustomerName, x.Carrier, x.TrackingNo));
    }

    public async Task<IEnumerable<CycleCountVarianceReportDto>> GetCycleCountVarianceReportAsync(ReportFilterDto filter, CancellationToken ct = default)
    {
        var query = _cycleCountRepo.Query()
            .Include(x => x.Item).Include(x => x.Location)
            .Where(x => !x.IsDeleted && x.CountedQty != x.SystemQty);

        if (filter.FromDate.HasValue) query = query.Where(x => x.CountDate >= filter.FromDate.Value);
        if (filter.ToDate.HasValue) query = query.Where(x => x.CountDate <= filter.ToDate.Value);

        var result = await query.OrderByDescending(x => x.CountDate).ToListAsync(ct);
        return result.Select(x => new CycleCountVarianceReportDto(
            x.CountNumber, x.CountDate, x.Item.ItemCode, x.Item.ItemName,
            x.Location.LocationCode, x.SystemQty, x.CountedQty, x.CountedQty - x.SystemQty));
    }
}
