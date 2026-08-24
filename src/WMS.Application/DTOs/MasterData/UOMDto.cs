namespace WMS.Application.DTOs.MasterData;

public record UOMDto(Guid Id, string UOMCode, string UOMName, decimal ConversionFactor, bool IsActive);

public record CreateUOMDto(string UOMCode, string UOMName, decimal ConversionFactor);

public record UpdateUOMDto(string UOMName, decimal ConversionFactor, bool IsActive);
