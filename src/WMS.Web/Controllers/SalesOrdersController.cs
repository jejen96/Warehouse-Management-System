using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Web.Models;
using WMS.Web.Services;

namespace WMS.Web.Controllers;

[Authorize]
public class SalesOrdersController : Controller
{
    private readonly ApiClient _api;
    public SalesOrdersController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(int page = 1, string? status = null)
    {
        var result = await _api.GetPagedAsync<SalesOrderDto>("sales-orders", page, 10);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result?.Data?.TotalPages ?? 1;
        ViewBag.TotalCount = result?.Data?.TotalCount ?? 0;
        ViewBag.Status = status;
        return View(result?.Data?.Items ?? Enumerable.Empty<SalesOrderDto>());
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _api.GetByIdAsync<SalesOrderDto>("sales-orders", id);
        if (result?.Data == null) return NotFound();
        return View(result.Data);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> UpdateStatus(Guid id, string status)
    {
        await _api.PutAsync<SalesOrderDto>($"sales-orders/{id}/status", id, new { status });
        TempData["Success"] = $"SO status updated to {status}.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
