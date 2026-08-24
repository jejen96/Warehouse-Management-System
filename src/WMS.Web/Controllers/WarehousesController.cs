using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Web.Models;
using WMS.Web.Services;

namespace WMS.Web.Controllers;

[Authorize]
public class WarehousesController : Controller
{
    private readonly ApiClient _api;
    public WarehousesController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _api.GetPagedAsync<WarehouseDto>("warehouses", page, 10);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result?.Data?.TotalPages ?? 1;
        return View(result?.Data?.Items ?? Enumerable.Empty<WarehouseDto>());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new WarehouseDto());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(WarehouseDto model)
    {
        var result = await _api.PostAsync<WarehouseDto>("warehouses",
            new { model.WarehouseCode, model.WarehouseName, model.Address });

        if (result?.Success == true) { TempData["Success"] = "Warehouse created."; return RedirectToAction(nameof(Index)); }
        TempData["Error"] = result?.Message; return View(model);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _api.GetByIdAsync<WarehouseDto>("warehouses", id);
        if (result?.Data == null) return NotFound();
        return View(result.Data);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id, WarehouseDto model)
    {
        var result = await _api.PutAsync<WarehouseDto>("warehouses", id,
            new { model.WarehouseName, model.Address, model.IsActive });

        if (result?.Success == true) { TempData["Success"] = "Warehouse updated."; return RedirectToAction(nameof(Index)); }
        TempData["Error"] = result?.Message; return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _api.DeleteAsync("warehouses", id);
        TempData["Success"] = "Warehouse deleted.";
        return RedirectToAction(nameof(Index));
    }
}
