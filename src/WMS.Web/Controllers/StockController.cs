using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Web.Models;
using WMS.Web.Services;

namespace WMS.Web.Controllers;

[Authorize]
public class StockController : Controller
{
    private readonly ApiClient _api;
    public StockController(ApiClient api) => _api = api;

    public async Task<IActionResult> Balance(int page = 1)
    {
        var result = await _api.GetPagedAsync<StockBalanceDto>("stock/balances", page, 15);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result?.Data?.TotalPages ?? 1;
        ViewBag.TotalCount = result?.Data?.TotalCount ?? 0;
        return View(result?.Data?.Items ?? Enumerable.Empty<StockBalanceDto>());
    }

    public async Task<IActionResult> Ledger(int page = 1)
    {
        var result = await _api.GetPagedAsync<StockLedgerDto>("stock/ledger", page, 15);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result?.Data?.TotalPages ?? 1;
        return View(result?.Data?.Items ?? Enumerable.Empty<StockLedgerDto>());
    }

    public async Task<IActionResult> Adjustments(int page = 1)
    {
        var result = await _api.GetPagedAsync<StockAdjustmentDto>("stock-adjustments", page, 10);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result?.Data?.TotalPages ?? 1;
        return View(result?.Data?.Items ?? Enumerable.Empty<StockAdjustmentDto>());
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Approve(Guid id)
    {
        await _api.PostAsync<StockAdjustmentDto>($"stock-adjustments/{id}/approve", new { });
        TempData["Success"] = "Adjustment approved.";
        return RedirectToAction(nameof(Adjustments));
    }
}
