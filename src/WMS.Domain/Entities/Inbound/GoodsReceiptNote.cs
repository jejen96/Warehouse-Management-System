using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities.Inbound;

public class GoodsReceiptNote : BaseEntity
{
    public string GRNNumber { get; set; } = string.Empty;
    public DateTime GRNDate { get; set; }
    public Guid POId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public string ReceivedBy { get; set; } = string.Empty;
    public GRNStatus Status { get; set; } = GRNStatus.Draft;
    public string? Notes { get; set; }

    public ICollection<GoodsReceiptNoteDetail> Details { get; set; } = new List<GoodsReceiptNoteDetail>();
}
