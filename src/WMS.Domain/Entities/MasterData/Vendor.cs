using WMS.Domain.Common;

namespace WMS.Domain.Entities.MasterData;

public class Vendor : BaseEntity
{
    public string VendorCode { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string? Contact { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
}
