using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;

namespace WMS.Domain.Entities.Outbound;

public class PickingListDetail : BaseEntity
{
    public Guid PickingListId { get; set; }
    public PickingList PickingList { get; set; } = null!;

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public Guid LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public decimal RequiredQty { get; set; }
    public decimal PickedQty { get; set; }
    public bool IsPicked { get; set; }
}
