using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;

namespace WMS.Domain.Entities.Inventory;

public class CycleCount : BaseEntity
{
    public string CountNumber { get; set; } = string.Empty;
    public DateTime CountDate { get; set; }

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public Guid LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public decimal SystemQty { get; set; }
    public decimal CountedQty { get; set; }
    public decimal Variance => CountedQty - SystemQty;
    public string? Notes { get; set; }
    public bool IsAdjusted { get; set; }
}
