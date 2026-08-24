using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.Inventory;
using WMS.Domain.Entities.Inventory;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Inventory;

public interface ICycleCountService
{
    Task<PagedResult<CycleCountDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default);
    Task<CycleCountDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CycleCountDto> CreateAsync(CreateCycleCountDto dto, string createdBy, CancellationToken ct = default);
    Task<CycleCountDto> AdjustAsync(Guid id, string adjustedBy, CancellationToken ct = default);
}

public class CycleCountService : ICycleCountService
{
    private readonly IRepository<CycleCount> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IStockService _stockService;

    public CycleCountService(IRepository<CycleCount> repo, IUnitOfWork uow, IMapper mapper, IStockService stockService)
    {
        _repo = repo; _uow = uow; _mapper = mapper; _stockService = stockService;
    }

    public async Task<PagedResult<CycleCountDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default)
    {
        var query = _repo.Query().Include(x => x.Item).Include(x => x.Location).Where(x => !x.IsDeleted);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.CountDate)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<CycleCountDto>
        {
            Items = _mapper.Map<IEnumerable<CycleCountDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<CycleCountDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cc = await _repo.Query().Include(x => x.Item).Include(x => x.Location)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(CycleCount), id);
        return _mapper.Map<CycleCountDto>(cc);
    }

    public async Task<CycleCountDto> CreateAsync(CreateCycleCountDto dto, string createdBy, CancellationToken ct = default)
    {
        var systemQty = await _stockService.GetStockBalanceAsync(dto.ItemId, dto.LocationId, ct);
        var cc = new CycleCount
        {
            CountNumber = $"CC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            CountDate = DateTime.UtcNow,
            ItemId = dto.ItemId,
            LocationId = dto.LocationId,
            SystemQty = systemQty,
            CountedQty = dto.CountedQty,
            Notes = dto.Notes,
            CreatedBy = createdBy
        };
        await _repo.AddAsync(cc, ct);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(cc.Id, ct);
    }

    public async Task<CycleCountDto> AdjustAsync(Guid id, string adjustedBy, CancellationToken ct = default)
    {
        var cc = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(CycleCount), id);
        if (cc.IsAdjusted) throw new BusinessException("Cycle count already adjusted.");

        var variance = cc.CountedQty - cc.SystemQty;
        if (variance != 0)
        {
            await _uow.BeginTransactionAsync(ct);
            try
            {
                await _stockService.UpdateStockAsync(cc.ItemId, cc.LocationId, variance,
                    StockMovementType.CycleCount, cc.CountNumber, adjustedBy, ct);
                cc.IsAdjusted = true;
                cc.UpdatedBy = adjustedBy; cc.UpdatedAt = DateTime.UtcNow;
                await _uow.SaveChangesAsync(ct);
                await _uow.CommitTransactionAsync(ct);
            }
            catch { await _uow.RollbackTransactionAsync(ct); throw; }
        }

        return await GetByIdAsync(id, ct);
    }
}
