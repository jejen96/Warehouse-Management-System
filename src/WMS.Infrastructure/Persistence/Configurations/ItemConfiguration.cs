using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities.MasterData;

namespace WMS.Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ItemCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ItemName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.UOM).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Category).HasMaxLength(100);
        builder.Property(x => x.MinStock).HasPrecision(18, 4);
        builder.Property(x => x.MaxStock).HasPrecision(18, 4);
        builder.HasIndex(x => x.ItemCode).IsUnique();
    }
}

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.WarehouseCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.WarehouseName).IsRequired().HasMaxLength(200);
        builder.HasIndex(x => x.WarehouseCode).IsUnique();
        builder.HasMany(x => x.Locations).WithOne(l => l.Warehouse).HasForeignKey(l => l.WarehouseId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LocationCode).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.LocationCode).IsUnique();
    }
}

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.VendorCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.VendorName).IsRequired().HasMaxLength(200);
        builder.HasIndex(x => x.VendorCode).IsUnique();
    }
}

public class UOMConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UOMCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.UOMName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ConversionFactor).HasPrecision(18, 6);
        builder.HasIndex(x => x.UOMCode).IsUnique();
    }
}
