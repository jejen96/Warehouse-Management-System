using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;

namespace WMS.Domain.Entities.Outbound;

public class SalesOrderDetail : BaseEntity
{
    public Guid SalesOrderId { get; set; }
    public SalesOrder SalesOrder { get; set; } = null!;

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public decimal OrderedQty { get; set; }
    public string UOM { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal PickedQty { get; set; }
}
