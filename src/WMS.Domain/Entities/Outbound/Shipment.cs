using WMS.Domain.Common;

namespace WMS.Domain.Entities.Outbound;

public class Shipment : BaseEntity
{
    public string ShipmentNumber { get; set; } = string.Empty;
    public Guid PackId { get; set; }
    public Packing Packing { get; set; } = null!;
    public string Carrier { get; set; } = string.Empty;
    public string? TrackingNo { get; set; }
    public DateTime ShippedDate { get; set; }
    public string? Notes { get; set; }
}
