using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.Outbound;
using WMS.Domain.Entities.Inventory;
using WMS.Domain.Entities.Outbound;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Outbound;

public interface IPickingService
{
    Task<PagedResult<PickingListDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default);
    Task<PickingListDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PickingListDto> CreateAsync(CreatePickingListDto dto, string createdBy, CancellationToken ct = default);
    Task<PickingListDto> CompletePickingAsync(Guid id, List<UpdatePickedQtyDto> updates, string updatedBy, CancellationToken ct = default);
}

public class PickingService : IPickingService
{
    private readonly IRepository<PickingList> _repo;
    private readonly IRepository<SalesOrder> _soRepo;
    private readonly IRepository<StockBalance> _stockRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IStockService _stockService;

    public PickingService(IRepository<PickingList> repo, IRepository<SalesOrder> soRepo,
        IRepository<StockBalance> stockRepo, IUnitOfWork uow, IMapper mapper, IStockService stockService)
    {
        _repo = repo; _soRepo = soRepo; _stockRepo = stockRepo; _uow = uow; _mapper = mapper; _stockService = stockService;
    }

    public async Task<PagedResult<PickingListDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default)
    {
        var query = _repo.Query()
            .Include(x => x.SalesOrder)
            .Include(x => x.Details).ThenInclude(d => d.Item)
            .Include(x => x.Details).ThenInclude(d => d.Location)
            .Where(x => !x.IsDeleted);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<PickingListDto>
        {
            Items = _mapper.Map<IEnumerable<PickingListDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<PickingListDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var pl = await _repo.Query()
            .Include(x => x.SalesOrder)
            .Include(x => x.Details).ThenInclude(d => d.Item)
            .Include(x => x.Details).ThenInclude(d => d.Location)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(PickingList), id);
        return _mapper.Map<PickingListDto>(pl);
    }

    public async Task<PickingListDto> CreateAsync(CreatePickingListDto dto, string createdBy, CancellationToken ct = default)
    {
        var so = await _soRepo.Query()
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == dto.SalesOrderId && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(SalesOrder), dto.SalesOrderId);

        if (so.Status != SOStatus.Confirmed) throw new BusinessException("SO must be Confirmed to create a picking list.");

        // Auto-assign locations from stock balances
        var pickingList = new PickingList
        {
            SalesOrderId = dto.SalesOrderId,
            AssignedPicker = dto.AssignedPicker,
            CreatedBy = createdBy
        };

        foreach (var soDetail in so.Details)
        {
            var stockBalances = await _stockRepo.Query()
                .Include(x => x.Location)
                .Where(x => x.ItemId == soDetail.ItemId && x.AvailableQty > 0 && !x.IsDeleted)
                .OrderByDescending(x => x.AvailableQty)
                .ToListAsync(ct);

            decimal remaining = soDetail.OrderedQty;
            foreach (var balance in stockBalances)
            {
                if (remaining <= 0) break;
                var pickQty = Math.Min(remaining, balance.AvailableQty);
                pickingList.Details.Add(new PickingListDetail
                {
                    ItemId = soDetail.ItemId,
                    LocationId = balance.LocationId,
                    RequiredQty = pickQty,
                    CreatedBy = createdBy
                });
                remaining -= pickQty;
            }
        }

        so.Status = SOStatus.Picking;
        so.UpdatedBy = createdBy; so.UpdatedAt = DateTime.UtcNow;

        await _repo.AddAsync(pickingList, ct);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(pickingList.Id, ct);
    }

    public async Task<PickingListDto> CompletePickingAsync(Guid id, List<UpdatePickedQtyDto> updates, string updatedBy, CancellationToken ct = default)
    {
        var pl = await _repo.Query()
            .Include(x => x.Details)
            .Include(x => x.SalesOrder).ThenInclude(s => s.Details)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(PickingList), id);

        if (pl.IsCompleted) throw new BusinessException("Picking list already completed.");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            foreach (var update in updates)
            {
                var detail = pl.Details.FirstOrDefault(d => d.Id == update.PickingDetailId)
                    ?? throw new NotFoundException("PickingDetail", update.PickingDetailId);

                detail.PickedQty = update.PickedQty;
                detail.IsPicked = update.PickedQty >= detail.RequiredQty;

                // Deduct from stock
                await _stockService.UpdateStockAsync(detail.ItemId, detail.LocationId, -update.PickedQty,
                    StockMovementType.Outbound, $"PICK-{pl.Id}", updatedBy, ct);
            }

            pl.IsCompleted = true; pl.CompletedAt = DateTime.UtcNow;
            pl.UpdatedBy = updatedBy; pl.UpdatedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
        }
        catch { await _uow.RollbackTransactionAsync(ct); throw; }

        return await GetByIdAsync(id, ct);
    }
}
