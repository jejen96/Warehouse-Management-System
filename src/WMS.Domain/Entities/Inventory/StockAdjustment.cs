using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;

namespace WMS.Domain.Entities.Inventory;

public class StockAdjustment : BaseEntity
{
    public string AdjNumber { get; set; } = string.Empty;
    public DateTime AdjDate { get; set; }
    public string Reason { get; set; } = string.Empty;

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public Guid LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public decimal AdjQty { get; set; }
    public string? ApprovedBy { get; set; }
    public bool IsApproved { get; set; }
}
