using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.Inbound;
using WMS.Domain.Entities.Inbound;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Inbound;

public interface IGRNService
{
    Task<PagedResult<GRNDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default);
    Task<GRNDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<GRNDto> CreateAsync(CreateGRNDto dto, string createdBy, CancellationToken ct = default);
    Task<GRNDto> CompleteAsync(Guid id, string updatedBy, CancellationToken ct = default);
}

public class GRNService : IGRNService
{
    private readonly IRepository<GoodsReceiptNote> _repo;
    private readonly IRepository<PurchaseOrder> _poRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IStockService _stockService;

    public GRNService(IRepository<GoodsReceiptNote> repo, IRepository<PurchaseOrder> poRepo,
        IUnitOfWork uow, IMapper mapper, IStockService stockService)
    {
        _repo = repo; _poRepo = poRepo; _uow = uow; _mapper = mapper; _stockService = stockService;
    }

    public async Task<PagedResult<GRNDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default)
    {
        var query = _repo.Query()
            .Include(x => x.PurchaseOrder)
            .Include(x => x.Details).ThenInclude(d => d.Item)
            .Include(x => x.Details).ThenInclude(d => d.PutAwayLocation)
            .Where(x => !x.IsDeleted);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.GRNDate)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<GRNDto>
        {
            Items = _mapper.Map<IEnumerable<GRNDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<GRNDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var grn = await _repo.Query()
            .Include(x => x.PurchaseOrder)
            .Include(x => x.Details).ThenInclude(d => d.Item)
            .Include(x => x.Details).ThenInclude(d => d.PutAwayLocation)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(GoodsReceiptNote), id);
        return _mapper.Map<GRNDto>(grn);
    }

    public async Task<GRNDto> CreateAsync(CreateGRNDto dto, string createdBy, CancellationToken ct = default)
    {
        var po = await _poRepo.GetByIdAsync(dto.POId, ct)
            ?? throw new NotFoundException(nameof(PurchaseOrder), dto.POId);
        if (po.Status == POStatus.Closed) throw new BusinessException("Cannot create GRN for a closed PO.");

        var grn = _mapper.Map<GoodsReceiptNote>(dto);
        grn.GRNNumber = $"GRN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        grn.CreatedBy = createdBy;
        foreach (var d in grn.Details) d.CreatedBy = createdBy;

        await _repo.AddAsync(grn, ct);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(grn.Id, ct);
    }

    public async Task<GRNDto> CompleteAsync(Guid id, string updatedBy, CancellationToken ct = default)
    {
        var grn = await _repo.Query()
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(GoodsReceiptNote), id);

        if (grn.Status != GRNStatus.Draft) throw new BusinessException("GRN is already completed or cancelled.");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            foreach (var detail in grn.Details.Where(d => d.QCStatus == QCStatus.Accepted && d.PutAwayLocationId.HasValue))
            {
                await _stockService.UpdateStockAsync(
                    detail.ItemId, detail.PutAwayLocationId!.Value,
                    detail.ReceivedQty, StockMovementType.Inbound,
                    grn.GRNNumber, updatedBy, ct);
            }

            grn.Status = GRNStatus.Completed;
            grn.UpdatedBy = updatedBy; grn.UpdatedAt = DateTime.UtcNow;
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
