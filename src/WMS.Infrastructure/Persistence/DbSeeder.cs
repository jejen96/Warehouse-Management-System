using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Entities.Security;
using WMS.Domain.Enums;

namespace WMS.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(WmsDbContext context)
    {
        await context.Database.MigrateAsync();

        // Seed UOMs
        if (!await context.UnitsOfMeasure.AnyAsync())
        {
            context.UnitsOfMeasure.AddRange(
                new UnitOfMeasure { UOMCode = "PCS", UOMName = "Pieces", ConversionFactor = 1, CreatedBy = "system" },
                new UnitOfMeasure { UOMCode = "KG", UOMName = "Kilogram", ConversionFactor = 1, CreatedBy = "system" },
                new UnitOfMeasure { UOMCode = "BOX", UOMName = "Box", ConversionFactor = 1, CreatedBy = "system" },
                new UnitOfMeasure { UOMCode = "LTR", UOMName = "Liter", ConversionFactor = 1, CreatedBy = "system" }
            );
        }

        // Seed Warehouse
        Warehouse? warehouse = null;
        if (!await context.Warehouses.AnyAsync())
        {
            warehouse = new Warehouse
            {
                WarehouseCode = "WH-001",
                WarehouseName = "Main Warehouse",
                Address = "123 Industrial Zone, City",
                CreatedBy = "system"
            };
            context.Warehouses.Add(warehouse);
            await context.SaveChangesAsync();

            // Seed Locations
            context.Locations.AddRange(
                new Location { LocationCode = "A-01-01", Aisle = "A", Rack = "01", Level = "01", WarehouseId = warehouse.Id, CreatedBy = "system" },
                new Location { LocationCode = "A-01-02", Aisle = "A", Rack = "01", Level = "02", WarehouseId = warehouse.Id, CreatedBy = "system" },
                new Location { LocationCode = "B-01-01", Aisle = "B", Rack = "01", Level = "01", WarehouseId = warehouse.Id, CreatedBy = "system" },
                new Location { LocationCode = "B-02-01", Aisle = "B", Rack = "02", Level = "01", WarehouseId = warehouse.Id, CreatedBy = "system" }
            );
        }

        // Seed Admin User
        if (!await context.Users.AnyAsync())
        {
            context.Users.Add(new User
            {
                Username = "admin",
                Email = "admin@wms.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                WarehouseId = warehouse?.Id,
                CreatedBy = "system"
            });
        }

        await context.SaveChangesAsync();
    }
}
