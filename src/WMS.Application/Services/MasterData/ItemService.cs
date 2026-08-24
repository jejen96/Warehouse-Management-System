using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.MasterData;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.MasterData;

public interface IItemService
{
    Task<PagedResult<ItemDto>> GetAllAsync(PaginationParams pagination, string? search, CancellationToken ct = default);
    Task<ItemDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ItemDto> CreateAsync(CreateItemDto dto, string createdBy, CancellationToken ct = default);
    Task<ItemDto> UpdateAsync(Guid id, UpdateItemDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default);
}

public class ItemService : IItemService
{
    private readonly IRepository<Item> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ItemService(IRepository<Item> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _uow = uow; _mapper = mapper;
    }

    public async Task<PagedResult<ItemDto>> GetAllAsync(PaginationParams pagination, string? search, CancellationToken ct = default)
    {
        var query = _repo.Query().Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.ItemCode.Contains(search) || x.ItemName.Contains(search));

        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.ItemCode)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(ct);

        return new PagedResult<ItemDto>
        {
            Items = _mapper.Map<IEnumerable<ItemDto>>(items),
            TotalCount = total,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<ItemDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Item), id);
        return _mapper.Map<ItemDto>(item);
    }

    public async Task<ItemDto> CreateAsync(CreateItemDto dto, string createdBy, CancellationToken ct = default)
    {
        var existing = await _repo.Query().FirstOrDefaultAsync(x => x.ItemCode == dto.ItemCode && !x.IsDeleted, ct);
        if (existing != null) throw new BusinessException($"Item code '{dto.ItemCode}' already exists.");

        var item = _mapper.Map<Item>(dto);
        item.CreatedBy = createdBy;
        await _repo.AddAsync(item, ct);
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<ItemDto>(item);
    }

    public async Task<ItemDto> UpdateAsync(Guid id, UpdateItemDto dto, string updatedBy, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Item), id);

        _mapper.Map(dto, item);
        item.UpdatedBy = updatedBy;
        item.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<ItemDto>(item);
    }

    public async Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Item), id);
        item.IsDeleted = true;
        item.IsActive = false;
        item.UpdatedBy = deletedBy;
        item.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }
}
