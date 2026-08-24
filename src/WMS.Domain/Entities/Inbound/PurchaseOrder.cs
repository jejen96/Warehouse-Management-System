using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities.Inbound;

public class PurchaseOrder : BaseEntity
{
    public string PONumber { get; set; } = string.Empty;
    public DateTime PODate { get; set; }
    public Guid VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;
    public POStatus Status { get; set; } = POStatus.Draft;
    public string? Notes { get; set; }

    public ICollection<PurchaseOrderDetail> Details { get; set; } = new List<PurchaseOrderDetail>();
    public ICollection<GoodsReceiptNote> GRNs { get; set; } = new List<GoodsReceiptNote>();
}
