namespace WMS.Application.DTOs.Outbound;

public record PickingListDto(Guid Id, Guid SalesOrderId, string SONumber, string AssignedPicker, bool IsCompleted, DateTime? CompletedAt, List<PickingDetailDto> Details);

public record PickingDetailDto(Guid Id, Guid ItemId, string ItemName, Guid LocationId, string LocationCode, decimal RequiredQty, decimal PickedQty, bool IsPicked);

public record CreatePickingListDto(Guid SalesOrderId, string AssignedPicker);

public record UpdatePickedQtyDto(Guid PickingDetailId, decimal PickedQty);
