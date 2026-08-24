using WMS.Domain.Common;
using WMS.Domain.Entities.MasterData;

namespace WMS.Domain.Entities.Outbound;

public class PickingList : BaseEntity
{
    public Guid SalesOrderId { get; set; }
    public SalesOrder SalesOrder { get; set; } = null!;

    public string AssignedPicker { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }

    public ICollection<PickingListDetail> Details { get; set; } = new List<PickingListDetail>();
}
