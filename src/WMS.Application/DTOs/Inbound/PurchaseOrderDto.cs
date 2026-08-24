using WMS.Domain.Enums;

namespace WMS.Application.DTOs.Inbound;

public record PurchaseOrderDto(Guid Id, string PONumber, DateTime PODate, Guid VendorId, string VendorName, POStatus Status, string? Notes, List<PODetailDto> Details);

public record PODetailDto(Guid Id, Guid ItemId, string ItemName, decimal OrderedQty, string UOM, decimal UnitPrice, decimal ReceivedQty);

public record CreatePODto(DateTime PODate, Guid VendorId, string? Notes, List<CreatePODetailDto> Details);

public record CreatePODetailDto(Guid ItemId, decimal OrderedQty, string UOM, decimal UnitPrice);

public record UpdatePOStatusDto(POStatus Status);
