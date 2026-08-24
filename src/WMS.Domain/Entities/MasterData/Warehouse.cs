using WMS.Domain.Common;

namespace WMS.Domain.Entities.MasterData;

public class Warehouse : BaseEntity
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Location> Locations { get; set; } = new List<Location>();
}
