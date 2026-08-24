using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.API.Extensions;
using WMS.Application.Common;

namespace WMS.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    protected string CurrentUser => User.GetUsername();

    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data, string message = "Success")
        => Ok(ApiResponse<T>.Ok(data, message));

    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(T data, string message = "Created successfully")
        => StatusCode(201, ApiResponse<T>.Ok(data, message));
}
