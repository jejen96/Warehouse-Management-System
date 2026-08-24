namespace WMS.Application.DTOs.MasterData;

public record ItemDto(Guid Id, string ItemCode, string ItemName, string? Description, string UOM, string? Category, decimal MinStock, decimal MaxStock, bool IsActive);

public record CreateItemDto(string ItemCode, string ItemName, string? Description, string UOM, string? Category, decimal MinStock, decimal MaxStock);

public record UpdateItemDto(string ItemName, string? Description, string UOM, string? Category, decimal MinStock, decimal MaxStock, bool IsActive);
