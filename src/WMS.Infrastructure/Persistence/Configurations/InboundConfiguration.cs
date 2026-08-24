using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities.Inbound;

namespace WMS.Infrastructure.Persistence.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PONumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.PONumber).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>();
        builder.HasOne(x => x.Vendor).WithMany().HasForeignKey(x => x.VendorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Details).WithOne(d => d.PurchaseOrder).HasForeignKey(d => d.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PurchaseOrderDetailConfiguration : IEntityTypeConfiguration<PurchaseOrderDetail>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderDetail> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OrderedQty).HasPrecision(18, 4);
        builder.Property(x => x.ReceivedQty).HasPrecision(18, 4);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 4);
        builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class GRNConfiguration : IEntityTypeConfiguration<GoodsReceiptNote>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptNote> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.GRNNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.GRNNumber).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>();
        builder.HasOne(x => x.PurchaseOrder).WithMany(p => p.GRNs).HasForeignKey(x => x.POId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Details).WithOne(d => d.GRN).HasForeignKey(d => d.GRNId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class GRNDetailConfiguration : IEntityTypeConfiguration<GoodsReceiptNoteDetail>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptNoteDetail> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReceivedQty).HasPrecision(18, 4);
        builder.Property(x => x.QCStatus).HasConversion<string>();
        builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PutAwayLocation).WithMany().HasForeignKey(x => x.PutAwayLocationId).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
    }
}
