using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;

namespace WMS.Domain.Entities.Inbound;

public class PurchaseOrderDetail : BaseEntity
{
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public decimal OrderedQty { get; set; }
    public string UOM { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal ReceivedQty { get; set; }
}
