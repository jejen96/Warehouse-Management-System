using WMS.Domain.Enums;

namespace WMS.Application.DTOs.Outbound;

public record SalesOrderDto(Guid Id, string SONumber, DateTime SODate, string CustomerName, string? CustomerAddress, SOStatus Status, string? Notes, List<SODetailDto> Details);

public record SODetailDto(Guid Id, Guid ItemId, string ItemName, decimal OrderedQty, string UOM, decimal UnitPrice, decimal PickedQty);

public record CreateSODto(DateTime SODate, string CustomerName, string? CustomerAddress, string? Notes, List<CreateSODetailDto> Details);

public record CreateSODetailDto(Guid ItemId, decimal OrderedQty, string UOM, decimal UnitPrice);

public record UpdateSOStatusDto(SOStatus Status);
