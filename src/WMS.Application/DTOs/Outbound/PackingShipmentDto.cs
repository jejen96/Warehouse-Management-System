namespace WMS.Application.DTOs.Outbound;

public record PackingDto(Guid Id, string PackNumber, Guid SalesOrderId, string SONumber, string PackedBy, DateTime PackedDate, string? Notes);

public record CreatePackingDto(Guid SalesOrderId, string PackedBy, DateTime PackedDate, string? Notes);

public record ShipmentDto(Guid Id, string ShipmentNumber, Guid PackId, string PackNumber, string Carrier, string? TrackingNo, DateTime ShippedDate, string? Notes);

public record CreateShipmentDto(Guid PackId, string Carrier, string? TrackingNo, DateTime ShippedDate, string? Notes);
