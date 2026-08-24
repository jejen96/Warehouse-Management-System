using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;

namespace WMS.Domain.Entities.Inventory;

public class StockTransfer : BaseEntity
{
    public string TransferNumber { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public Guid FromLocationId { get; set; }
    public Location FromLocation { get; set; } = null!;

    public Guid ToLocationId { get; set; }
    public Location ToLocation { get; set; } = null!;

    public decimal Qty { get; set; }
    public string? Notes { get; set; }
}
