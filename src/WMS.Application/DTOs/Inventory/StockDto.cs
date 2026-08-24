using WMS.Domain.Enums;

namespace WMS.Application.DTOs.Inventory;

public record StockBalanceDto(Guid ItemId, string ItemCode, string ItemName, Guid LocationId, string LocationCode, string WarehouseName, decimal AvailableQty, decimal ReservedQty, decimal OnHandQty);

public record StockLedgerDto(Guid Id, Guid ItemId, string ItemName, Guid LocationId, string LocationCode, decimal Quantity, StockMovementType MovementType, string ReferenceNumber, string? Remarks, DateTime CreatedAt);

public record CreateStockAdjustmentDto(Guid ItemId, Guid LocationId, decimal AdjQty, string Reason);

public record StockAdjustmentDto(Guid Id, string AdjNumber, DateTime AdjDate, Guid ItemId, string ItemName, Guid LocationId, string LocationCode, decimal AdjQty, string Reason, bool IsApproved, string? ApprovedBy);

public record CreateStockTransferDto(Guid ItemId, Guid FromLocationId, Guid ToLocationId, decimal Qty, string? Notes);

public record StockTransferDto(Guid Id, string TransferNumber, DateTime TransferDate, Guid ItemId, string ItemName, Guid FromLocationId, string FromLocationCode, Guid ToLocationId, string ToLocationCode, decimal Qty);

public record CreateCycleCountDto(Guid ItemId, Guid LocationId, decimal CountedQty, string? Notes);

public record CycleCountDto(Guid Id, string CountNumber, DateTime CountDate, Guid ItemId, string ItemName, Guid LocationId, string LocationCode, decimal SystemQty, decimal CountedQty, decimal Variance, bool IsAdjusted);
