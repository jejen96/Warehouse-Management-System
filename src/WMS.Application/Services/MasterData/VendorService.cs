using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.MasterData;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.MasterData;

public interface IVendorService
{
    Task<PagedResult<VendorDto>> GetAllAsync(PaginationParams pagination, string? search, CancellationToken ct = default);
    Task<VendorDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<VendorDto> CreateAsync(CreateVendorDto dto, string createdBy, CancellationToken ct = default);
    Task<VendorDto> UpdateAsync(Guid id, UpdateVendorDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default);
}

public class VendorService : IVendorService
{
    private readonly IRepository<Vendor> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public VendorService(IRepository<Vendor> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _uow = uow; _mapper = mapper;
    }

    public async Task<PagedResult<VendorDto>> GetAllAsync(PaginationParams pagination, string? search, CancellationToken ct = default)
    {
        var query = _repo.Query().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.VendorCode.Contains(search) || x.VendorName.Contains(search));

        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.VendorCode)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<VendorDto>
        {
            Items = _mapper.Map<IEnumerable<VendorDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<VendorDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var v = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Vendor), id);
        return _mapper.Map<VendorDto>(v);
    }

    public async Task<VendorDto> CreateAsync(CreateVendorDto dto, string createdBy, CancellationToken ct = default)
    {
        var existing = await _repo.Query().FirstOrDefaultAsync(x => x.VendorCode == dto.VendorCode && !x.IsDeleted, ct);
        if (existing != null) throw new BusinessException($"Vendor code '{dto.VendorCode}' already exists.");

        var v = _mapper.Map<Vendor>(dto);
        v.CreatedBy = createdBy;
        await _repo.AddAsync(v, ct);
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<VendorDto>(v);
    }

    public async Task<VendorDto> UpdateAsync(Guid id, UpdateVendorDto dto, string updatedBy, CancellationToken ct = default)
    {
        var v = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Vendor), id);
        _mapper.Map(dto, v);
        v.UpdatedBy = updatedBy; v.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<VendorDto>(v);
    }

    public async Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default)
    {
        var v = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Vendor), id);
        v.IsDeleted = true; v.IsActive = false;
        v.UpdatedBy = deletedBy; v.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }
}
