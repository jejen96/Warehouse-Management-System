using WMS.Domain.Common;

namespace WMS.Domain.Entities.MasterData;

public class Location : BaseEntity
{
    public string LocationCode { get; set; } = string.Empty;
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Level { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
}
