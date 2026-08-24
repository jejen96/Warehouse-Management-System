using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.Inbound;
using WMS.Domain.Entities.Inbound;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Inbound;

public interface IPurchaseOrderService
{
    Task<PagedResult<PurchaseOrderDto>> GetAllAsync(PaginationParams pagination, POStatus? status, CancellationToken ct = default);
    Task<PurchaseOrderDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PurchaseOrderDto> CreateAsync(CreatePODto dto, string createdBy, CancellationToken ct = default);
    Task<PurchaseOrderDto> UpdateStatusAsync(Guid id, UpdatePOStatusDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default);
}

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IRepository<PurchaseOrder> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PurchaseOrderService(IRepository<PurchaseOrder> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _uow = uow; _mapper = mapper;
    }

    public async Task<PagedResult<PurchaseOrderDto>> GetAllAsync(PaginationParams pagination, POStatus? status, CancellationToken ct = default)
    {
        var query = _repo.Query()
            .Include(x => x.Vendor)
            .Include(x => x.Details).ThenInclude(d => d.Item)
            .Where(x => !x.IsDeleted);

        if (status.HasValue) query = query.Where(x => x.Status == status.Value);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.PODate)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<PurchaseOrderDto>
        {
            Items = _mapper.Map<IEnumerable<PurchaseOrderDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<PurchaseOrderDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var po = await _repo.Query()
            .Include(x => x.Vendor)
            .Include(x => x.Details).ThenInclude(d => d.Item)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(PurchaseOrder), id);
        return _mapper.Map<PurchaseOrderDto>(po);
    }

    public async Task<PurchaseOrderDto> CreateAsync(CreatePODto dto, string createdBy, CancellationToken ct = default)
    {
        var po = _mapper.Map<PurchaseOrder>(dto);
        po.PONumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        po.CreatedBy = createdBy;
        foreach (var d in po.Details) d.CreatedBy = createdBy;

        await _repo.AddAsync(po, ct);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(po.Id, ct);
    }

    public async Task<PurchaseOrderDto> UpdateStatusAsync(Guid id, UpdatePOStatusDto dto, string updatedBy, CancellationToken ct = default)
    {
        var po = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(PurchaseOrder), id);
        po.Status = dto.Status;
        po.UpdatedBy = updatedBy; po.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default)
    {
        var po = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(PurchaseOrder), id);
        if (po.Status != POStatus.Draft) throw new BusinessException("Only Draft POs can be deleted.");
        po.IsDeleted = true; po.UpdatedBy = deletedBy; po.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }
}
