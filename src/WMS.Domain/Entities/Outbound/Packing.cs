using WMS.Domain.Common;

namespace WMS.Domain.Entities.Outbound;

public class Packing : BaseEntity
{
    public string PackNumber { get; set; } = string.Empty;
    public Guid SalesOrderId { get; set; }
    public SalesOrder SalesOrder { get; set; } = null!;
    public string PackedBy { get; set; } = string.Empty;
    public DateTime PackedDate { get; set; }
    public string? Notes { get; set; }

    public Shipment? Shipment { get; set; }
}
