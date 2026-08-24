using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Web.Models;
using WMS.Web.Services;

namespace WMS.Web.Controllers;

[Authorize]
public class PurchaseOrdersController : Controller
{
    private readonly ApiClient _api;
    public PurchaseOrdersController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(int page = 1, string? status = null)
    {
        var endpoint = status != null ? $"purchase-orders?status={status}" : "purchase-orders";
        var result = await _api.GetPagedAsync<PurchaseOrderDto>("purchase-orders", page, 10);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result?.Data?.TotalPages ?? 1;
        ViewBag.TotalCount = result?.Data?.TotalCount ?? 0;
        ViewBag.Status = status;
        return View(result?.Data?.Items ?? Enumerable.Empty<PurchaseOrderDto>());
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _api.GetByIdAsync<PurchaseOrderDto>("purchase-orders", id);
        if (result?.Data == null) return NotFound();
        return View(result.Data);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> UpdateStatus(Guid id, string status)
    {
        await _api.PutAsync<PurchaseOrderDto>($"purchase-orders/{id}/status", id, new { status });
        TempData["Success"] = $"PO status updated to {status}.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
