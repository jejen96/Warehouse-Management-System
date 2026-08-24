using WMS.Domain.Enums;

namespace WMS.Application.DTOs.Inbound;

public record GRNDto(Guid Id, string GRNNumber, DateTime GRNDate, Guid POId, string PONumber, string ReceivedBy, GRNStatus Status, string? Notes, List<GRNDetailDto> Details);

public record GRNDetailDto(Guid Id, Guid ItemId, string ItemName, decimal ReceivedQty, QCStatus QCStatus, Guid? PutAwayLocationId, string? PutAwayLocationCode);

public record CreateGRNDto(DateTime GRNDate, Guid POId, string ReceivedBy, string? Notes, List<CreateGRNDetailDto> Details);

public record CreateGRNDetailDto(Guid ItemId, decimal ReceivedQty, QCStatus QCStatus, Guid? PutAwayLocationId);
