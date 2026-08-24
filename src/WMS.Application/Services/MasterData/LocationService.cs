using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.MasterData;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.MasterData;

public interface ILocationService
{
    Task<PagedResult<LocationDto>> GetAllAsync(PaginationParams pagination, Guid? warehouseId, CancellationToken ct = default);
    Task<LocationDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<LocationDto> CreateAsync(CreateLocationDto dto, string createdBy, CancellationToken ct = default);
    Task<LocationDto> UpdateAsync(Guid id, UpdateLocationDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default);
}

public class LocationService : ILocationService
{
    private readonly IRepository<Location> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public LocationService(IRepository<Location> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _uow = uow; _mapper = mapper;
    }

    public async Task<PagedResult<LocationDto>> GetAllAsync(PaginationParams pagination, Guid? warehouseId, CancellationToken ct = default)
    {
        var query = _repo.Query().Include(x => x.Warehouse).Where(x => !x.IsDeleted);
        if (warehouseId.HasValue) query = query.Where(x => x.WarehouseId == warehouseId.Value);

        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.LocationCode)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<LocationDto>
        {
            Items = _mapper.Map<IEnumerable<LocationDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<LocationDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var loc = await _repo.Query().Include(x => x.Warehouse).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(Location), id);
        return _mapper.Map<LocationDto>(loc);
    }

    public async Task<LocationDto> CreateAsync(CreateLocationDto dto, string createdBy, CancellationToken ct = default)
    {
        var loc = _mapper.Map<Location>(dto);
        loc.CreatedBy = createdBy;
        await _repo.AddAsync(loc, ct);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(loc.Id, ct);
    }

    public async Task<LocationDto> UpdateAsync(Guid id, UpdateLocationDto dto, string updatedBy, CancellationToken ct = default)
    {
        var loc = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Location), id);
        loc.Aisle = dto.Aisle; loc.Rack = dto.Rack; loc.Level = dto.Level; loc.IsActive = dto.IsActive;
        loc.UpdatedBy = updatedBy; loc.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default)
    {
        var loc = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Location), id);
        loc.IsDeleted = true; loc.IsActive = false;
        loc.UpdatedBy = deletedBy; loc.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }
}
