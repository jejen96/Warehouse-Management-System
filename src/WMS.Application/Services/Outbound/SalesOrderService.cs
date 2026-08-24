using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.Outbound;
using WMS.Domain.Entities.Outbound;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Outbound;

public interface ISalesOrderService
{
    Task<PagedResult<SalesOrderDto>> GetAllAsync(PaginationParams pagination, SOStatus? status, CancellationToken ct = default);
    Task<SalesOrderDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SalesOrderDto> CreateAsync(CreateSODto dto, string createdBy, CancellationToken ct = default);
    Task<SalesOrderDto> UpdateStatusAsync(Guid id, UpdateSOStatusDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default);
}

public class SalesOrderService : ISalesOrderService
{
    private readonly IRepository<SalesOrder> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public SalesOrderService(IRepository<SalesOrder> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _uow = uow; _mapper = mapper;
    }

    public async Task<PagedResult<SalesOrderDto>> GetAllAsync(PaginationParams pagination, SOStatus? status, CancellationToken ct = default)
    {
        var query = _repo.Query()
            .Include(x => x.Details).ThenInclude(d => d.Item)
            .Where(x => !x.IsDeleted);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.SODate)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<SalesOrderDto>
        {
            Items = _mapper.Map<IEnumerable<SalesOrderDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<SalesOrderDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var so = await _repo.Query()
            .Include(x => x.Details).ThenInclude(d => d.Item)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(SalesOrder), id);
        return _mapper.Map<SalesOrderDto>(so);
    }

    public async Task<SalesOrderDto> CreateAsync(CreateSODto dto, string createdBy, CancellationToken ct = default)
    {
        var so = _mapper.Map<SalesOrder>(dto);
        so.SONumber = $"SO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        so.CreatedBy = createdBy;
        foreach (var d in so.Details) d.CreatedBy = createdBy;

        await _repo.AddAsync(so, ct);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(so.Id, ct);
    }

    public async Task<SalesOrderDto> UpdateStatusAsync(Guid id, UpdateSOStatusDto dto, string updatedBy, CancellationToken ct = default)
    {
        var so = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(SalesOrder), id);
        so.Status = dto.Status;
        so.UpdatedBy = updatedBy; so.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default)
    {
        var so = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(SalesOrder), id);
        if (so.Status != SOStatus.Draft) throw new BusinessException("Only Draft SOs can be deleted.");
        so.IsDeleted = true; so.UpdatedBy = deletedBy; so.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }
}
