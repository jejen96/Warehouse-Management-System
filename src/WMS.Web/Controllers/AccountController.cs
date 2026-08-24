using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.Web.Models;
using WMS.Web.Services;

namespace WMS.Web.Controllers;

public class AccountController : Controller
{
    private readonly ApiClient _api;

    public AccountController(ApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _api.LoginAsync(model.Username, model.Password);

        if (result == null || !result.Success || result.Data == null)
        {
            ModelState.AddModelError("", result?.Message ?? "Login failed. Please check your credentials.");
            return View(model);
        }

        // Store JWT token in a secure HTTP-only cookie (most reliable)
        Response.Cookies.Append("WmsJwtToken", result.Data.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,      // set true in production with HTTPS
            SameSite = SameSiteMode.Lax,
            Expires = result.Data.ExpiresAt
        });

        // Also keep in Session as backup
        HttpContext.Session.SetString("JwtToken", result.Data.Token);

        // Create cookie auth claims for MVC authorization
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, result.Data.Username),
            new(ClaimTypes.Email, result.Data.Email),
            new(ClaimTypes.Role, result.Data.Role),
            new("JwtToken", result.Data.Token)   // store in claims too
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = result.Data.ExpiresAt
            });

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Dashboard");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        Response.Cookies.Delete("WmsJwtToken");
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();
}
