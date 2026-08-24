using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WMS.Web.Models;

namespace WMS.Web.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient http, IHttpContextAccessor httpContextAccessor)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    // Read token: Claims -> Cookie -> Session
    private string? GetToken()
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx == null) return null;

        // 1. From Claims (most reliable — stored during SignIn)
        var fromClaims = ctx.User?.FindFirst("JwtToken")?.Value;
        if (!string.IsNullOrEmpty(fromClaims)) return fromClaims;

        // 2. From Cookie
        if (ctx.Request.Cookies.TryGetValue("WmsJwtToken", out var fromCookie)
            && !string.IsNullOrEmpty(fromCookie)) return fromCookie;

        // 3. From Session fallback
        return ctx.Session.GetString("JwtToken");
    }

    // Build per-request message with Authorization header
    private HttpRequestMessage BuildRequest(HttpMethod method, string url, HttpContent? content = null)
    {
        var req = new HttpRequestMessage(method, url);
        var token = GetToken();
        if (!string.IsNullOrEmpty(token))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (content != null)
            req.Content = content;
        return req;
    }

    private StringContent ToJson<T>(T obj) =>
        new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    private T? SafeDeserialize<T>(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return default;
        try { return JsonSerializer.Deserialize<T>(content, _jsonOptions); }
        catch { return default; }
    }

    // ── Auth (no token needed) ──────────────────────────────────────────────
    public async Task<ApiResponse<LoginResponse>?> LoginAsync(string username, string password)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, "auth/login")
        {
            Content = ToJson(new { username, password })
        };
        var resp = await _http.SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();
        return SafeDeserialize<ApiResponse<LoginResponse>>(body);
    }

    // ── GET paged ───────────────────────────────────────────────────────────
    public async Task<ApiResponse<PagedResult<T>>?> GetPagedAsync<T>(
        string endpoint, int page = 1, int pageSize = 10, string query = "")
    {
        var url = $"{endpoint}?pageNumber={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(query)) url += $"&search={Uri.EscapeDataString(query)}";

        var resp = await _http.SendAsync(BuildRequest(HttpMethod.Get, url));

        if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            return new ApiResponse<PagedResult<T>> { Success = false, Message = $"HTTP {(int)resp.StatusCode} - Token may have expired. Please logout and login again." };

        var body = await resp.Content.ReadAsStringAsync();
        return SafeDeserialize<ApiResponse<PagedResult<T>>>(body);
    }

    // ── GET by id ───────────────────────────────────────────────────────────
    public async Task<ApiResponse<T>?> GetByIdAsync<T>(string endpoint, Guid id)
    {
        var resp = await _http.SendAsync(BuildRequest(HttpMethod.Get, $"{endpoint}/{id}"));
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            return new ApiResponse<T> { Success = false, Message = "Unauthorized" };
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return new ApiResponse<T> { Success = false, Message = "Not found" };
        var body = await resp.Content.ReadAsStringAsync();
        return SafeDeserialize<ApiResponse<T>>(body);
    }

    // ── POST ────────────────────────────────────────────────────────────────
    public async Task<ApiResponse<T>?> PostAsync<T>(string endpoint, object body)
    {
        var resp = await _http.SendAsync(BuildRequest(HttpMethod.Post, endpoint, ToJson(body)));
        var content = await resp.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(content))
            return new ApiResponse<T> { Success = false, Message = $"API error: HTTP {(int)resp.StatusCode}" };

        var result = SafeDeserialize<ApiResponse<T>>(content);
        return result ?? new ApiResponse<T> { Success = false, Message = content[..Math.Min(300, content.Length)] };
    }

    // ── PUT ─────────────────────────────────────────────────────────────────
    public async Task<ApiResponse<T>?> PutAsync<T>(string endpoint, Guid id, object body)
    {
        var resp = await _http.SendAsync(BuildRequest(HttpMethod.Put, $"{endpoint}/{id}", ToJson(body)));
        var content = await resp.Content.ReadAsStringAsync();
        return SafeDeserialize<ApiResponse<T>>(content);
    }

    // ── PUT sub-resource (e.g. status) ──────────────────────────────────────
    public async Task<ApiResponse<T>?> PutSubAsync<T>(string fullEndpoint, object body)
    {
        var resp = await _http.SendAsync(BuildRequest(HttpMethod.Put, fullEndpoint, ToJson(body)));
        var content = await resp.Content.ReadAsStringAsync();
        return SafeDeserialize<ApiResponse<T>>(content);
    }

    // ── DELETE ──────────────────────────────────────────────────────────────
    public async Task<ApiResponse<object>?> DeleteAsync(string endpoint, Guid id)
    {
        var resp = await _http.SendAsync(BuildRequest(HttpMethod.Delete, $"{endpoint}/{id}"));
        var content = await resp.Content.ReadAsStringAsync();
        return SafeDeserialize<ApiResponse<object>>(content);
    }

    // ── GET list (no paging) ────────────────────────────────────────────────
    public async Task<ApiResponse<IEnumerable<T>>?> GetListAsync<T>(string endpoint)
    {
        var resp = await _http.SendAsync(BuildRequest(HttpMethod.Get, endpoint));
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            return new ApiResponse<IEnumerable<T>> { Success = false, Message = "Unauthorized" };
        var body = await resp.Content.ReadAsStringAsync();
        return SafeDeserialize<ApiResponse<IEnumerable<T>>>(body);
    }
}
