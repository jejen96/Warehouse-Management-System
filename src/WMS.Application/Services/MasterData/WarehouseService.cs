using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.MasterData;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.MasterData;

public interface IWarehouseService
{
    Task<PagedResult<WarehouseDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default);
    Task<WarehouseDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto, string createdBy, CancellationToken ct = default);
    Task<WarehouseDto> UpdateAsync(Guid id, UpdateWarehouseDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default);
}

public class WarehouseService : IWarehouseService
{
    private readonly IRepository<Warehouse> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public WarehouseService(IRepository<Warehouse> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _uow = uow; _mapper = mapper;
    }

    public async Task<PagedResult<WarehouseDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default)
    {
        var query = _repo.Query().Where(x => !x.IsDeleted);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.WarehouseCode)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<WarehouseDto>
        {
            Items = _mapper.Map<IEnumerable<WarehouseDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<WarehouseDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var wh = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Warehouse), id);
        return _mapper.Map<WarehouseDto>(wh);
    }

    public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto, string createdBy, CancellationToken ct = default)
    {
        var existing = await _repo.Query().FirstOrDefaultAsync(x => x.WarehouseCode == dto.WarehouseCode && !x.IsDeleted, ct);
        if (existing != null) throw new BusinessException($"Warehouse code '{dto.WarehouseCode}' already exists.");

        var wh = _mapper.Map<Warehouse>(dto);
        wh.CreatedBy = createdBy;
        await _repo.AddAsync(wh, ct);
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<WarehouseDto>(wh);
    }

    public async Task<WarehouseDto> UpdateAsync(Guid id, UpdateWarehouseDto dto, string updatedBy, CancellationToken ct = default)
    {
        var wh = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Warehouse), id);
        _mapper.Map(dto, wh);
        wh.UpdatedBy = updatedBy;
        wh.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<WarehouseDto>(wh);
    }

    public async Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default)
    {
        var wh = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Warehouse), id);
        wh.IsDeleted = true; wh.IsActive = false;
        wh.UpdatedBy = deletedBy; wh.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }
}
