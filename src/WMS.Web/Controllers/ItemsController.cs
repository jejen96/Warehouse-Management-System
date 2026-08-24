using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Web.Models;
using WMS.Web.Services;

namespace WMS.Web.Controllers;

[Authorize]
public class ItemsController : Controller
{
    private readonly ApiClient _api;
    public ItemsController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        var result = await _api.GetPagedAsync<ItemDto>("items", page, 10, search ?? "");
        ViewBag.Search = search;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result?.Data?.TotalPages ?? 1;
        ViewBag.TotalCount = result?.Data?.TotalCount ?? 0;
        return View(result?.Data?.Items ?? Enumerable.Empty<ItemDto>());
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _api.GetByIdAsync<ItemDto>("items", id);
        if (result?.Data == null) return NotFound();
        return View(result.Data);
    }

    [Authorize(Roles = "Admin,WarehouseManager")]
    public IActionResult Create() => View(new ItemDto());

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Create(ItemDto model)
    {
        var result = await _api.PostAsync<ItemDto>("items", new
        {
            model.ItemCode, model.ItemName, model.Description,
            model.UOM, model.Category, model.MinStock, model.MaxStock
        });

        if (result?.Success == true)
        {
            TempData["Success"] = $"Item '{model.ItemName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Show detailed error from API including validation messages
        var errorMsg = result?.Message ?? "Failed to create item.";
        if (result?.Errors?.Any() == true)
            errorMsg += " " + string.Join("; ", result.Errors);

        ModelState.AddModelError("", errorMsg);
        ViewBag.ApiError = errorMsg;
        return View(model);
    }

    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _api.GetByIdAsync<ItemDto>("items", id);
        if (result?.Data == null) return NotFound();
        return View(result.Data);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Edit(Guid id, ItemDto model)
    {
        var result = await _api.PutAsync<ItemDto>("items", id, new
        {
            model.ItemName, model.Description, model.UOM,
            model.Category, model.MinStock, model.MaxStock, model.IsActive
        });

        if (result?.Success == true)
        {
            TempData["Success"] = "Item updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = result?.Message ?? "Failed to update item.";
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _api.DeleteAsync("items", id);
        TempData["Success"] = "Item deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
