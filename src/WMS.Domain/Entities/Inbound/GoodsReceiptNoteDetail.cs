using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities.Inbound;

public class GoodsReceiptNoteDetail : BaseEntity
{
    public Guid GRNId { get; set; }
    public GoodsReceiptNote GRN { get; set; } = null!;

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public decimal ReceivedQty { get; set; }
    public QCStatus QCStatus { get; set; } = QCStatus.Pending;

    public Guid? PutAwayLocationId { get; set; }
    public Location? PutAwayLocation { get; set; }
}
