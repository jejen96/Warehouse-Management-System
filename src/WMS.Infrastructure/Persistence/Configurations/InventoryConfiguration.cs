using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities.Inventory;

namespace WMS.Infrastructure.Persistence.Configurations;

public class StockBalanceConfiguration : IEntityTypeConfiguration<StockBalance>
{
    public void Configure(EntityTypeBuilder<StockBalance> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AvailableQty).HasPrecision(18, 4);
        builder.Property(x => x.ReservedQty).HasPrecision(18, 4);
        builder.Ignore(x => x.OnHandQty);
        builder.HasIndex(x => new { x.ItemId, x.LocationId }).IsUnique();
        builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockLedgerConfiguration : IEntityTypeConfiguration<StockLedger>
{
    public void Configure(EntityTypeBuilder<StockLedger> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.MovementType).HasConversion<string>();
        builder.Property(x => x.ReferenceNumber).IsRequired().HasMaxLength(100);
        builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AdjNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.AdjQty).HasPrecision(18, 4);
        builder.HasIndex(x => x.AdjNumber).IsUnique();
        builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class StockTransferConfiguration : IEntityTypeConfiguration<StockTransfer>
{
    public void Configure(EntityTypeBuilder<StockTransfer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TransferNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Qty).HasPrecision(18, 4);
        builder.HasIndex(x => x.TransferNumber).IsUnique();
        builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FromLocation).WithMany().HasForeignKey(x => x.FromLocationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ToLocation).WithMany().HasForeignKey(x => x.ToLocationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CycleCountConfiguration : IEntityTypeConfiguration<CycleCount>
{
    public void Configure(EntityTypeBuilder<CycleCount> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CountNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.SystemQty).HasPrecision(18, 4);
        builder.Property(x => x.CountedQty).HasPrecision(18, 4);
        builder.Ignore(x => x.Variance);
        builder.HasIndex(x => x.CountNumber).IsUnique();
        builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
    }
}
