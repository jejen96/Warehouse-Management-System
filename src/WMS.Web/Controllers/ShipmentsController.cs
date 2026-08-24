using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Web.Models;
using WMS.Web.Services;

namespace WMS.Web.Controllers;

[Authorize]
public class ShipmentsController : Controller
{
    private readonly ApiClient _api;
    public ShipmentsController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await _api.GetPagedAsync<ShipmentDto>("shipments", page, 10);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result?.Data?.TotalPages ?? 1;
        return View(result?.Data?.Items ?? Enumerable.Empty<ShipmentDto>());
    }
}
