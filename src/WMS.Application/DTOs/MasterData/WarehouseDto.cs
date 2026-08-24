namespace WMS.Application.DTOs.MasterData;

public record WarehouseDto(Guid Id, string WarehouseCode, string WarehouseName, string? Address, bool IsActive);

public record CreateWarehouseDto(string WarehouseCode, string WarehouseName, string? Address);

public record UpdateWarehouseDto(string WarehouseName, string? Address, bool IsActive);
