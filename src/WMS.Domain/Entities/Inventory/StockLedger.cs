using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities.Inventory;

public class StockLedger : BaseEntity
{
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public Guid LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public decimal Quantity { get; set; }
    public StockMovementType MovementType { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}
