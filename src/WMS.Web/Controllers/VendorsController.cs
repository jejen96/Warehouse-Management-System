using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Web.Models;
using WMS.Web.Services;

namespace WMS.Web.Controllers;

[Authorize]
public class VendorsController : Controller
{
    private readonly ApiClient _api;
    public VendorsController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        var result = await _api.GetPagedAsync<VendorDto>("vendors", page, 10, search ?? "");
        ViewBag.Search = search;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result?.Data?.TotalPages ?? 1;
        return View(result?.Data?.Items ?? Enumerable.Empty<VendorDto>());
    }

    [Authorize(Roles = "Admin,WarehouseManager")]
    public IActionResult Create() => View(new VendorDto());

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Create(VendorDto model)
    {
        var result = await _api.PostAsync<VendorDto>("vendors",
            new { model.VendorCode, model.VendorName, model.Contact, model.Address });
        if (result?.Success == true) { TempData["Success"] = "Vendor created."; return RedirectToAction(nameof(Index)); }
        TempData["Error"] = result?.Message; return View(model);
    }

    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _api.GetByIdAsync<VendorDto>("vendors", id);
        if (result?.Data == null) return NotFound();
        return View(result.Data);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Edit(Guid id, VendorDto model)
    {
        var result = await _api.PutAsync<VendorDto>("vendors", id,
            new { model.VendorName, model.Contact, model.Address, model.IsActive });
        if (result?.Success == true) { TempData["Success"] = "Vendor updated."; return RedirectToAction(nameof(Index)); }
        TempData["Error"] = result?.Message; return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _api.DeleteAsync("vendors", id);
        TempData["Success"] = "Vendor deleted.";
        return RedirectToAction(nameof(Index));
    }
}
