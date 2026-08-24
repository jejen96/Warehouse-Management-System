using WMS.Domain.Common;

namespace WMS.Domain.Entities.MasterData;

public class Item : BaseEntity
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string UOM { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal MinStock { get; set; }
    public decimal MaxStock { get; set; }
    public bool IsActive { get; set; } = true;
}
