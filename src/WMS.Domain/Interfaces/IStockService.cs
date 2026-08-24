using WMS.Domain.Enums;

namespace WMS.Domain.Interfaces;

public interface IStockService
{
    Task UpdateStockAsync(Guid itemId, Guid locationId, decimal qty, StockMovementType movementType, string referenceNumber, string updatedBy, CancellationToken ct = default);
    Task<decimal> GetStockBalanceAsync(Guid itemId, Guid locationId, CancellationToken ct = default);
}
