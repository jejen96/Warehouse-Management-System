using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.Inventory;
using WMS.Domain.Entities.Inventory;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Inventory;

public interface IStockAdjustmentService
{
    Task<PagedResult<StockAdjustmentDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default);
    Task<StockAdjustmentDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<StockAdjustmentDto> CreateAsync(CreateStockAdjustmentDto dto, string createdBy, CancellationToken ct = default);
    Task<StockAdjustmentDto> ApproveAsync(Guid id, string approvedBy, CancellationToken ct = default);
}

public class StockAdjustmentService : IStockAdjustmentService
{
    private readonly IRepository<StockAdjustment> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IStockService _stockService;

    public StockAdjustmentService(IRepository<StockAdjustment> repo, IUnitOfWork uow, IMapper mapper, IStockService stockService)
    {
        _repo = repo; _uow = uow; _mapper = mapper; _stockService = stockService;
    }

    public async Task<PagedResult<StockAdjustmentDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default)
    {
        var query = _repo.Query()
            .Include(x => x.Item).Include(x => x.Location)
            .Where(x => !x.IsDeleted);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.AdjDate)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<StockAdjustmentDto>
        {
            Items = _mapper.Map<IEnumerable<StockAdjustmentDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<StockAdjustmentDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var adj = await _repo.Query().Include(x => x.Item).Include(x => x.Location)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(StockAdjustment), id);
        return _mapper.Map<StockAdjustmentDto>(adj);
    }

    public async Task<StockAdjustmentDto> CreateAsync(CreateStockAdjustmentDto dto, string createdBy, CancellationToken ct = default)
    {
        var adj = new StockAdjustment
        {
            AdjNumber = $"ADJ-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            AdjDate = DateTime.UtcNow,
            ItemId = dto.ItemId,
            LocationId = dto.LocationId,
            AdjQty = dto.AdjQty,
            Reason = dto.Reason,
            CreatedBy = createdBy
        };
        await _repo.AddAsync(adj, ct);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(adj.Id, ct);
    }

    public async Task<StockAdjustmentDto> ApproveAsync(Guid id, string approvedBy, CancellationToken ct = default)
    {
        var adj = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(StockAdjustment), id);
        if (adj.IsApproved) throw new BusinessException("Adjustment already approved.");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            await _stockService.UpdateStockAsync(adj.ItemId, adj.LocationId, adj.AdjQty,
                StockMovementType.Adjustment, adj.AdjNumber, approvedBy, ct);

            adj.IsApproved = true; adj.ApprovedBy = approvedBy;
            adj.UpdatedBy = approvedBy; adj.UpdatedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }

        return await GetByIdAsync(id, ct);
    }
}
