using Microsoft.EntityFrameworkCore;
using WMS.Application.Common;
using WMS.Application.DTOs.Security;
using WMS.Domain.Entities.Security;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Security;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default);
    Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default);
    Task ChangePasswordAsync(Guid id, ChangePasswordDto dto, CancellationToken ct = default);
}

public class UserService : IUserService
{
    private readonly IRepository<User> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IRepository<User> repo, IUnitOfWork uow, IPasswordHasher passwordHasher)
    {
        _repo = repo; _uow = uow; _passwordHasher = passwordHasher;
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(PaginationParams pagination, CancellationToken ct = default)
    {
        var query = _repo.Query().Where(x => !x.IsDeleted);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Username)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize).ToListAsync(ct);

        return new PagedResult<UserDto>
        {
            Items = items.Select(u => new UserDto(u.Id, u.Username, u.Email, u.Role, u.WarehouseId, u.IsActive)),
            TotalCount = total, PageNumber = pagination.PageNumber, PageSize = pagination.PageSize
        };
    }

    public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var u = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(User), id);
        return new UserDto(u.Id, u.Username, u.Email, u.Role, u.WarehouseId, u.IsActive);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto, string updatedBy, CancellationToken ct = default)
    {
        var u = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(User), id);
        u.Email = dto.Email; u.Role = dto.Role; u.WarehouseId = dto.WarehouseId; u.IsActive = dto.IsActive;
        u.UpdatedBy = updatedBy; u.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
        return new UserDto(u.Id, u.Username, u.Email, u.Role, u.WarehouseId, u.IsActive);
    }

    public async Task DeleteAsync(Guid id, string deletedBy, CancellationToken ct = default)
    {
        var u = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(User), id);
        u.IsDeleted = true; u.IsActive = false;
        u.UpdatedBy = deletedBy; u.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task ChangePasswordAsync(Guid id, ChangePasswordDto dto, CancellationToken ct = default)
    {
        var u = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(User), id);
        if (!_passwordHasher.Verify(dto.CurrentPassword, u.PasswordHash))
            throw new BusinessException("Current password is incorrect.");

        u.PasswordHash = _passwordHasher.Hash(dto.NewPassword);
        u.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }
}
