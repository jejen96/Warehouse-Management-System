namespace WMS.Application.DTOs.Reports;

public record StockBalanceReportDto(string ItemCode, string ItemName, string LocationCode, string WarehouseName, decimal AvailableQty, decimal ReservedQty, decimal OnHandQty);

public record StockMovementReportDto(DateTime Date, string ItemCode, string ItemName, string LocationCode, string MovementType, string ReferenceNumber, decimal Quantity, string CreatedBy);

public record GRNReportDto(string GRNNumber, DateTime GRNDate, string PONumber, string VendorName, string ReceivedBy, string Status, int TotalLines, decimal TotalQty);

public record ShipmentReportDto(string ShipmentNumber, DateTime ShippedDate, string SONumber, string CustomerName, string Carrier, string? TrackingNo);

public record CycleCountVarianceReportDto(string CountNumber, DateTime CountDate, string ItemCode, string ItemName, string LocationCode, decimal SystemQty, decimal CountedQty, decimal Variance);

public record ReportFilterDto(DateTime? FromDate, DateTime? ToDate, Guid? WarehouseId, Guid? ItemId, Guid? LocationId);
