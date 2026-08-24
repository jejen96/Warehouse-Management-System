using WMS.Domain.Common;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities.Security;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Operator;
    public Guid? WarehouseId { get; set; }
    public bool IsActive { get; set; } = true;
}
