using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Web.Models;
using WMS.Web.Services;

namespace WMS.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApiClient _api;

    public DashboardController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var stats = new DashboardStats();

        try
        {
            var items = await _api.GetPagedAsync<ItemDto>("items", 1, 1);
            stats.TotalItems = items?.Data?.TotalCount ?? 0;

            var warehouses = await _api.GetPagedAsync<WarehouseDto>("warehouses", 1, 1);
            stats.TotalWarehouses = warehouses?.Data?.TotalCount ?? 0;

            var vendors = await _api.GetPagedAsync<VendorDto>("vendors", 1, 1);
            stats.TotalVendors = vendors?.Data?.TotalCount ?? 0;

            var locations = await _api.GetPagedAsync<LocationDto>("locations", 1, 1);
            stats.TotalLocations = locations?.Data?.TotalCount ?? 0;

            var pendingPOs = await _api.GetPagedAsync<PurchaseOrderDto>("purchase-orders?status=Draft", 1, 1);
            stats.PendingPOs = pendingPOs?.Data?.TotalCount ?? 0;

            var pendingSOs = await _api.GetPagedAsync<SalesOrderDto>("sales-orders?status=Confirmed", 1, 1);
            stats.PendingSOs = pendingSOs?.Data?.TotalCount ?? 0;

            var recentPOs = await _api.GetPagedAsync<PurchaseOrderDto>("purchase-orders", 1, 5);
            stats.RecentPOs = recentPOs?.Data?.Items.ToList() ?? new();

            var recentSOs = await _api.GetPagedAsync<SalesOrderDto>("sales-orders", 1, 5);
            stats.RecentSOs = recentSOs?.Data?.Items.ToList() ?? new();

            var stockBalance = await _api.GetPagedAsync<StockBalanceDto>("stock/balances", 1, 5);
            stats.LowStockItems = stockBalance?.Data?.Items.ToList() ?? new();
        }
        catch { /* API not reachable, show empty stats */ }

        return View(stats);
    }
}
