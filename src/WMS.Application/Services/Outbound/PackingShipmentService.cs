using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.Outbound;
using WMS.Domain.Entities.Outbound;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Outbound;

public interface IPackingService
{
    Task<PagedResult<PackingDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default);
    Task<PackingDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PackingDto> CreateAsync(CreatePackingDto dto, string createdBy, CancellationToken ct = default);
}

public interface IShipmentService
{
    Task<PagedResult<ShipmentDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default);
    Task<ShipmentDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ShipmentDto> CreateAsync(CreateShipmentDto dto, string createdBy, CancellationToken ct = default);
}

public class PackingService : IPackingService
{
    private readonly IRepository<Packing> _repo;
    private readonly IRepository<SalesOrder> _soRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PackingService(IRepository<Packing> repo, IRepository<SalesOrder> soRepo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _soRepo = soRepo; _uow = uow; _mapper = mapper;
    }

    public async Task<PagedResult<PackingDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default)
    {
        var query = _repo.Query().Include(x => x.SalesOrder).Where(x => !x.IsDeleted);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.PackedDate)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<PackingDto>
        {
            Items = _mapper.Map<IEnumerable<PackingDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<PackingDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var p = await _repo.Query().Include(x => x.SalesOrder)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(Packing), id);
        return _mapper.Map<PackingDto>(p);
    }

    public async Task<PackingDto> CreateAsync(CreatePackingDto dto, string createdBy, CancellationToken ct = default)
    {
        var so = await _soRepo.GetByIdAsync(dto.SalesOrderId, ct)
            ?? throw new NotFoundException(nameof(SalesOrder), dto.SalesOrderId);

        var packing = new Packing
        {
            PackNumber = $"PACK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            SalesOrderId = dto.SalesOrderId,
            PackedBy = dto.PackedBy,
            PackedDate = dto.PackedDate,
            Notes = dto.Notes,
            CreatedBy = createdBy
        };

        await _repo.AddAsync(packing, ct);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(packing.Id, ct);
    }
}

public class ShipmentService : IShipmentService
{
    private readonly IRepository<Shipment> _repo;
    private readonly IRepository<Packing> _packRepo;
    private readonly IRepository<SalesOrder> _soRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ShipmentService(IRepository<Shipment> repo, IRepository<Packing> packRepo,
        IRepository<SalesOrder> soRepo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _packRepo = packRepo; _soRepo = soRepo; _uow = uow; _mapper = mapper;
    }

    public async Task<PagedResult<ShipmentDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default)
    {
        var query = _repo.Query().Include(x => x.Packing).ThenInclude(p => p.SalesOrder).Where(x => !x.IsDeleted);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.ShippedDate)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<ShipmentDto>
        {
            Items = _mapper.Map<IEnumerable<ShipmentDto>>(items),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<ShipmentDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _repo.Query().Include(x => x.Packing).ThenInclude(p => p.SalesOrder)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(Shipment), id);
        return _mapper.Map<ShipmentDto>(s);
    }

    public async Task<ShipmentDto> CreateAsync(CreateShipmentDto dto, string createdBy, CancellationToken ct = default)
    {
        var pack = await _packRepo.Query().Include(x => x.SalesOrder)
            .FirstOrDefaultAsync(x => x.Id == dto.PackId && !x.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(Packing), dto.PackId);

        var shipment = new Shipment
        {
            ShipmentNumber = $"SHP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            PackId = dto.PackId,
            Carrier = dto.Carrier,
            TrackingNo = dto.TrackingNo,
            ShippedDate = dto.ShippedDate,
            Notes = dto.Notes,
            CreatedBy = createdBy
        };

        // Update SO status to Shipped
        pack.SalesOrder.Status = SOStatus.Shipped;
        pack.SalesOrder.UpdatedBy = createdBy; pack.SalesOrder.UpdatedAt = DateTime.UtcNow;

        await _repo.AddAsync(shipment, ct);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(shipment.Id, ct);
    }
}
