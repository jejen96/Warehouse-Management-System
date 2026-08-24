using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Web.Models;
using WMS.Web.Services;

namespace WMS.Web.Controllers;

[Authorize]
[Route("[controller]")]
public class GRNController : Controller
{
    private readonly ApiClient _api;
    public GRNController(ApiClient api) => _api = api;

    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _api.GetPagedAsync<GRNDto>("grn", page, 10);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result?.Data?.TotalPages ?? 1;
        ViewBag.TotalCount = result?.Data?.TotalCount ?? 0;
        return View(result?.Data?.Items ?? Enumerable.Empty<GRNDto>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _api.GetByIdAsync<GRNDto>("grn", id);
        if (result?.Data == null) return NotFound();
        return View(result.Data);
    }

    [HttpPost("{id}/complete"), ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Complete(Guid id)
    {
        await _api.PostAsync<GRNDto>($"grn/{id}/complete", new { });
        TempData["Success"] = "GRN completed and stock updated.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
