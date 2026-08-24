namespace WMS.Application.DTOs.MasterData;

public record VendorDto(Guid Id, string VendorCode, string VendorName, string? Contact, string? Address, bool IsActive);

public record CreateVendorDto(string VendorCode, string VendorName, string? Contact, string? Address);

public record UpdateVendorDto(string VendorName, string? Contact, string? Address, bool IsActive);
