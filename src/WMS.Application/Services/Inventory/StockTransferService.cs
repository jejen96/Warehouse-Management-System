using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.Inventory;
using WMS.Domain.Entities.Inventory;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Inventory;

public interface IStockTransferService
{
    Task<PagedResult<StockTransferDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default);
    Task<StockTransferDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<StockTransferDto> CreateAsync(CreateStockTransferDto dto, string createdBy, CancellationToken ct = default);
}

public class StockTransferService : IStockTransferService
{
    private readonly IRepository<StockTransfer> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IStockService _stockService;

    public StockTransferService(IRepository<StockTransfer> repo, IUnitOfWork uow, IMapper mapper, IStockService stockService)
    {
        _repo = repo; _uow = uow; _mapper = mapper; _stockService = stockService;
    }

    public async Task<PagedResult<StockTransferDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default)
    {
        var query = _repo.Query()
            .Include(x => x.Item).Include(x => x.FromLocation).Include(x => x.ToLocation)
            .Where(x => !x.IsDeleted);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.TransferDate)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<StockTransferDto>
        {
            Items = _mapper.Map<IEnumerable<StockTransferDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<StockTransferDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var t = await _repo.Query()
            .Include(x => x.Item).Include(x => x.FromLocation).Include(x => x.ToLocation)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(StockTransfer), id);
        return _mapper.Map<StockTransferDto>(t);
    }

    public async Task<StockTransferDto> CreateAsync(CreateStockTransferDto dto, string createdBy, CancellationToken ct = default)
    {
        if (dto.FromLocationId == dto.ToLocationId) throw new BusinessException("Source and destination locations must be different.");

        var balance = await _stockService.GetStockBalanceAsync(dto.ItemId, dto.FromLocationId, ct);
        if (balance < dto.Qty) throw new BusinessException($"Insufficient stock. Available: {balance}, Requested: {dto.Qty}");

        var transfer = new StockTransfer
        {
            TransferNumber = $"TRF-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            TransferDate = DateTime.UtcNow,
            ItemId = dto.ItemId,
            FromLocationId = dto.FromLocationId,
            ToLocationId = dto.ToLocationId,
            Qty = dto.Qty,
            Notes = dto.Notes,
            CreatedBy = createdBy
        };

        await _uow.BeginTransactionAsync(ct);
        try
        {
            await _stockService.UpdateStockAsync(dto.ItemId, dto.FromLocationId, -dto.Qty, StockMovementType.Transfer, transfer.TransferNumber, createdBy, ct);
            await _stockService.UpdateStockAsync(dto.ItemId, dto.ToLocationId, dto.Qty, StockMovementType.Transfer, transfer.TransferNumber, createdBy, ct);

            await _repo.AddAsync(transfer, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }

        return await GetByIdAsync(transfer.Id, ct);
    }
}
