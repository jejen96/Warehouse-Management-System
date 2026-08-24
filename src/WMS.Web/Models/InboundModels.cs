namespace WMS.Web.Models;

public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public DateTime PODate { get; set; }
    public Guid VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<PODetailDto> Details { get; set; } = new();
}

public class PODetailDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal OrderedQty { get; set; }
    public string UOM { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal ReceivedQty { get; set; }
}

public class GRNDto
{
    public Guid Id { get; set; }
    public string GRNNumber { get; set; } = string.Empty;
    public DateTime GRNDate { get; set; }
    public string PONumber { get; set; } = string.Empty;
    public string ReceivedBy { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<GRNDetailDto> Details { get; set; } = new();
}

public class GRNDetailDto
{
    public Guid Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal ReceivedQty { get; set; }
    public string QCStatus { get; set; } = string.Empty;
    public string? PutAwayLocationCode { get; set; }
}
