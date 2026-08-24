namespace WMS.Application.DTOs.MasterData;

public record LocationDto(Guid Id, string LocationCode, string? Aisle, string? Rack, string? Level, Guid WarehouseId, string WarehouseName, bool IsActive);

public record CreateLocationDto(string LocationCode, string? Aisle, string? Rack, string? Level, Guid WarehouseId);

public record UpdateLocationDto(string? Aisle, string? Rack, string? Level, bool IsActive);
