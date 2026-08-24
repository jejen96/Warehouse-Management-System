namespace WMS.Web.Models;

public class SalesOrderDto
{
    public Guid Id { get; set; }
    public string SONumber { get; set; } = string.Empty;
    public DateTime SODate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerAddress { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<SODetailDto> Details { get; set; } = new();
}

public class SODetailDto
{
    public Guid Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal OrderedQty { get; set; }
    public string UOM { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal PickedQty { get; set; }
}

public class ShipmentDto
{
    public Guid Id { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
    public string PackNumber { get; set; } = string.Empty;
    public string Carrier { get; set; } = string.Empty;
    public string? TrackingNo { get; set; }
    public DateTime ShippedDate { get; set; }
}

public class DashboardStats
{
    public int TotalItems { get; set; }
    public int TotalWarehouses { get; set; }
    public int PendingPOs { get; set; }
    public int PendingSOs { get; set; }
    public int TotalLocations { get; set; }
    public int TotalVendors { get; set; }
    public decimal TotalStockValue { get; set; }
    public List<StockBalanceDto> LowStockItems { get; set; } = new();
    public List<PurchaseOrderDto> RecentPOs { get; set; } = new();
    public List<SalesOrderDto> RecentSOs { get; set; } = new();
}
