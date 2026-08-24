using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WMS.Application.Mappings;
using WMS.Application.Services.Inbound;
using WMS.Application.Services.Inventory;
using WMS.Application.Services.MasterData;
using WMS.Application.Services.Outbound;
using WMS.Application.Services.Reports;
using WMS.Application.Services.Security;

namespace WMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Master Data
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IVendorService, VendorService>();

        // Inbound
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        services.AddScoped<IGRNService, GRNService>();

        // Inventory
        services.AddScoped<IStockQueryService, StockQueryService>();
        services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();
        services.AddScoped<IStockTransferService, StockTransferService>();
        services.AddScoped<ICycleCountService, CycleCountService>();

        // Outbound
        services.AddScoped<ISalesOrderService, SalesOrderService>();
        services.AddScoped<IPickingService, PickingService>();
        services.AddScoped<IPackingService, PackingService>();
        services.AddScoped<IShipmentService, ShipmentService>();

        // Reports
        services.AddScoped<IReportService, ReportService>();

        // Security
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
