using WMS.Domain.Common;
using WMS.Domain.Enums;

namespace WMS.Domain.Entities.Outbound;

public class SalesOrder : BaseEntity
{
    public string SONumber { get; set; } = string.Empty;
    public DateTime SODate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerAddress { get; set; }
    public SOStatus Status { get; set; } = SOStatus.Draft;
    public string? Notes { get; set; }

    public ICollection<SalesOrderDetail> Details { get; set; } = new List<SalesOrderDetail>();
    public ICollection<PickingList> PickingLists { get; set; } = new List<PickingList>();
}
