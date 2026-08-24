using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities.Inventory;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Persistence;

namespace WMS.Infrastructure.Services;

public class StockService : IStockService
{
    private readonly WmsDbContext _context;

    public StockService(WmsDbContext context) => _context = context;

    public async Task UpdateStockAsync(Guid itemId, Guid locationId, decimal qty, StockMovementType movementType,
        string referenceNumber, string updatedBy, CancellationToken ct = default)
    {
        // Update or create stock balance
        var balance = await _context.StockBalances
            .FirstOrDefaultAsync(x => x.ItemId == itemId && x.LocationId == locationId, ct);

        if (balance == null)
        {
            balance = new StockBalance
            {
                ItemId = itemId,
                LocationId = locationId,
                AvailableQty = qty,
                ReservedQty = 0,
                CreatedBy = updatedBy
            };
            await _context.StockBalances.AddAsync(balance, ct);
        }
        else
        {
            balance.AvailableQty += qty;
            balance.UpdatedBy = updatedBy;
            balance.UpdatedAt = DateTime.UtcNow;
        }

        // Record ledger entry
        var ledger = new StockLedger
        {
            ItemId = itemId,
            LocationId = locationId,
            Quantity = qty,
            MovementType = movementType,
            ReferenceNumber = referenceNumber,
            CreatedBy = updatedBy
        };
        await _context.StockLedgers.AddAsync(ledger, ct);
    }

    public async Task<decimal> GetStockBalanceAsync(Guid itemId, Guid locationId, CancellationToken ct = default)
    {
        var balance = await _context.StockBalances
            .FirstOrDefaultAsync(x => x.ItemId == itemId && x.LocationId == locationId, ct);
        return balance?.AvailableQty ?? 0;
    }
}
