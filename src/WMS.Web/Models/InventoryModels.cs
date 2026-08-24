namespace WMS.Web.Models;

public class StockBalanceDto
{
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public decimal AvailableQty { get; set; }
    public decimal ReservedQty { get; set; }
    public decimal OnHandQty { get; set; }
}

public class StockLedgerDto
{
    public Guid Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class StockAdjustmentDto
{
    public Guid Id { get; set; }
    public string AdjNumber { get; set; } = string.Empty;
    public DateTime AdjDate { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public decimal AdjQty { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string? ApprovedBy { get; set; }
}
