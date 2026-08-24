using WMS.Application.Common;
using WMS.Application.DTOs.Security;
using WMS.Domain.Entities.Security;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services.Security;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<UserDto> RegisterAsync(CreateUserDto dto, string createdBy, CancellationToken ct = default);
}

public interface ITokenService
{
    string GenerateToken(User user);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _uow;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IRepository<User> userRepo, IUnitOfWork uow, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _userRepo = userRepo; _uow = uow; _tokenService = tokenService; _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = _userRepo.Query()
            .FirstOrDefault(u => u.Username == dto.Username && !u.IsDeleted && u.IsActive)
            ?? throw new BusinessException("Invalid username or password.");

        if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
            throw new BusinessException("Invalid username or password.");

        var token = _tokenService.GenerateToken(user);
        return new LoginResponseDto(token, user.Username, user.Email, user.Role, DateTime.UtcNow.AddHours(8));
    }

    public async Task<UserDto> RegisterAsync(CreateUserDto dto, string createdBy, CancellationToken ct = default)
    {
        var existing = _userRepo.Query().FirstOrDefault(u => u.Username == dto.Username && !u.IsDeleted);
        if (existing != null) throw new BusinessException($"Username '{dto.Username}' already exists.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            Role = dto.Role,
            WarehouseId = dto.WarehouseId,
            CreatedBy = createdBy
        };

        await _userRepo.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return new UserDto(user.Id, user.Username, user.Email, user.Role, user.WarehouseId, user.IsActive);
    }
}
