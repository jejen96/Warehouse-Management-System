using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;

namespace WMS.Domain.Entities.Inventory;

public class StockBalance : BaseEntity
{
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public Guid LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public decimal AvailableQty { get; set; }
    public decimal ReservedQty { get; set; }
    public decimal OnHandQty => AvailableQty + ReservedQty;
}
