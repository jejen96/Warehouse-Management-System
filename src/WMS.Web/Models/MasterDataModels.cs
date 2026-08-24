namespace WMS.Web.Models;

public class ItemDto
{
    public Guid Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string UOM { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal MinStock { get; set; }
    public decimal MaxStock { get; set; }
    public bool IsActive { get; set; }
}

public class WarehouseDto
{
    public Guid Id { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; }
}

public class LocationDto
{
    public Guid Id { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string? Aisle { get; set; }
    public string? Rack { get; set; }
    public string? Level { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class VendorDto
{
    public Guid Id { get; set; }
    public string VendorCode { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string? Contact { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
}
