using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.Inventory;
using WMS.Domain.Entities.Inventory;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Inventory;

public interface IStockQueryService
{
    Task<PagedResult<StockBalanceDto>> GetStockBalancesAsync(PaginationParams pagination, Guid? itemId, Guid? locationId, Guid? warehouseId, CancellationToken ct = default);
    Task<PagedResult<StockLedgerDto>> GetStockLedgerAsync(PaginationParams pagination, Guid? itemId, Guid? locationId, DateTime? from, DateTime? to, CancellationToken ct = default);
}

public class StockQueryService : IStockQueryService
{
    private readonly IRepository<StockBalance> _balanceRepo;
    private readonly IRepository<StockLedger> _ledgerRepo;
    private readonly IMapper _mapper;

    public StockQueryService(IRepository<StockBalance> balanceRepo, IRepository<StockLedger> ledgerRepo, IMapper mapper)
    {
        _balanceRepo = balanceRepo; _ledgerRepo = ledgerRepo; _mapper = mapper;
    }

    public async Task<PagedResult<StockBalanceDto>> GetStockBalancesAsync(PaginationParams pagination, Guid? itemId, Guid? locationId, Guid? warehouseId, CancellationToken ct = default)
    {
        var query = _balanceRepo.Query()
            .Include(x => x.Item)
            .Include(x => x.Location).ThenInclude(l => l.Warehouse)
            .Where(x => !x.IsDeleted && x.AvailableQty > 0);

        if (itemId.HasValue) query = query.Where(x => x.ItemId == itemId.Value);
        if (locationId.HasValue) query = query.Where(x => x.LocationId == locationId.Value);
        if (warehouseId.HasValue) query = query.Where(x => x.Location.WarehouseId == warehouseId.Value);

        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Item.ItemCode)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<StockBalanceDto>
        {
            Items = _mapper.Map<IEnumerable<StockBalanceDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<PagedResult<StockLedgerDto>> GetStockLedgerAsync(PaginationParams pagination, Guid? itemId, Guid? locationId, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var query = _ledgerRepo.Query()
            .Include(x => x.Item).Include(x => x.Location)
            .Where(x => !x.IsDeleted);

        if (itemId.HasValue) query = query.Where(x => x.ItemId == itemId.Value);
        if (locationId.HasValue) query = query.Where(x => x.LocationId == locationId.Value);
        if (from.HasValue) query = query.Where(x => x.CreatedAt >= from.Value);
        if (to.HasValue) query = query.Where(x => x.CreatedAt <= to.Value);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<StockLedgerDto>
        {
            Items = _mapper.Map<IEnumerable<StockLedgerDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }
}
