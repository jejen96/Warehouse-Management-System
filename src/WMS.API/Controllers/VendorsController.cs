using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.MasterData;
using WMS.Application.Services.MasterData;

namespace WMS.API.Controllers;

/// <summary>Vendor master data management</summary>
public class VendorsController : BaseController
{
    private readonly IVendorService _service;
    public VendorsController(IVendorService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<VendorDto>>>> GetAll(
        [FromQuery] PaginationParams pagination, [FromQuery] string? search, CancellationToken ct)
        => OkResponse(await _service.GetAllAsync(pagination, search, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<VendorDto>>> GetById(Guid id, CancellationToken ct)
        => OkResponse(await _service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<ActionResult<ApiResponse<VendorDto>>> Create([FromBody] CreateVendorDto dto, CancellationToken ct)
        => CreatedResponse(await _service.CreateAsync(dto, CurrentUser, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<ActionResult<ApiResponse<VendorDto>>> Update(Guid id, [FromBody] UpdateVendorDto dto, CancellationToken ct)
        => OkResponse(await _service.UpdateAsync(id, dto, CurrentUser, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, CurrentUser, ct);
        return OkResponse<object>(null!, "Vendor deleted.");
    }
}
