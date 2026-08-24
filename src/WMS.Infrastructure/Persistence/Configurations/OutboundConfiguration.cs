using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities.Outbound;

namespace WMS.Infrastructure.Persistence.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SONumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.SONumber).IsUnique();
        builder.Property(x => x.Status).HasConversion<string>();
        builder.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
        builder.HasMany(x => x.Details).WithOne(d => d.SalesOrder).HasForeignKey(d => d.SalesOrderId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SalesOrderDetailConfiguration : IEntityTypeConfiguration<SalesOrderDetail>
{
    public void Configure(EntityTypeBuilder<SalesOrderDetail> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OrderedQty).HasPrecision(18, 4);
        builder.Property(x => x.PickedQty).HasPrecision(18, 4);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 4);
        builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PickingListConfiguration : IEntityTypeConfiguration<PickingList>
{
    public void Configure(EntityTypeBuilder<PickingList> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.SalesOrder).WithMany(s => s.PickingLists).HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Details).WithOne(d => d.PickingList).HasForeignKey(d => d.PickingListId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PickingListDetailConfiguration : IEntityTypeConfiguration<PickingListDetail>
{
    public void Configure(EntityTypeBuilder<PickingListDetail> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RequiredQty).HasPrecision(18, 4);
        builder.Property(x => x.PickedQty).HasPrecision(18, 4);
        builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Location).WithMany().HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PackingConfiguration : IEntityTypeConfiguration<Packing>
{
    public void Configure(EntityTypeBuilder<Packing> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PackNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.PackNumber).IsUnique();
        builder.HasOne(x => x.SalesOrder).WithMany().HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Shipment).WithOne(s => s.Packing).HasForeignKey<Shipment>(s => s.PackId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ShipmentNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.ShipmentNumber).IsUnique();
    }
}
