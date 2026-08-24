using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities.Inbound;
using WMS.Domain.Entities.Inventory;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Entities.Outbound;
using WMS.Domain.Entities.Security;

namespace WMS.Infrastructure.Persistence;

public class WmsDbContext : DbContext
{
    public WmsDbContext(DbContextOptions<WmsDbContext> options) : base(options) { }

    // Master Data
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();

    // Inbound
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();
    public DbSet<GoodsReceiptNote> GoodsReceiptNotes => Set<GoodsReceiptNote>();
    public DbSet<GoodsReceiptNoteDetail> GoodsReceiptNoteDetails => Set<GoodsReceiptNoteDetail>();

    // Inventory
    public DbSet<StockBalance> StockBalances => Set<StockBalance>();
    public DbSet<StockLedger> StockLedgers => Set<StockLedger>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<CycleCount> CycleCounts => Set<CycleCount>();

    // Outbound
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderDetail> SalesOrderDetails => Set<SalesOrderDetail>();
    public DbSet<PickingList> PickingLists => Set<PickingList>();
    public DbSet<PickingListDetail> PickingListDetails => Set<PickingListDetail>();
    public DbSet<Packing> Packings => Set<Packing>();
    public DbSet<Shipment> Shipments => Set<Shipment>();

    // Security
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WmsDbContext).Assembly);

        // Global soft-delete query filter
        modelBuilder.Entity<Item>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Warehouse>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Location>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Vendor>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<UnitOfMeasure>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<PurchaseOrder>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<PurchaseOrderDetail>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<GoodsReceiptNote>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<GoodsReceiptNoteDetail>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<StockBalance>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<StockLedger>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<StockAdjustment>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<StockTransfer>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<CycleCount>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<SalesOrder>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<SalesOrderDetail>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<PickingList>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<PickingListDetail>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Packing>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Shipment>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
    }
}
