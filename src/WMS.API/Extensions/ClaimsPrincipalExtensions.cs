using System.Security.Claims;

namespace WMS.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue("unique_name") ?? "unknown";

    public static Guid GetUserId(this ClaimsPrincipal user)
        => Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub"), out var id) ? id : Guid.Empty;

    public static string GetRole(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
