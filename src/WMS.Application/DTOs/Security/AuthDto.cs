using WMS.Domain.Enums;

namespace WMS.Application.DTOs.Security;

public record LoginDto(string Username, string Password);

public record LoginResponseDto(string Token, string Username, string Email, UserRole Role, DateTime ExpiresAt);

public record UserDto(Guid Id, string Username, string Email, UserRole Role, Guid? WarehouseId, bool IsActive);

public record CreateUserDto(string Username, string Email, string Password, UserRole Role, Guid? WarehouseId);

public record UpdateUserDto(string Email, UserRole Role, Guid? WarehouseId, bool IsActive);

public record ChangePasswordDto(string CurrentPassword, string NewPassword);
