
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SupportAudit.Api.Infrastructure;

public class RequestUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public string? UserId =>
        User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User?.FindFirst("sub")?.Value
        ?? User?.FindFirst("userId")?.Value
        ?? User?.FindFirst("UserId")?.Value;

    public string? Role =>
        User?.FindFirst(ClaimTypes.Role)?.Value
        ?? User?.FindFirst("role")?.Value
        ?? User?.FindFirst("roles")?.Value;

    public string? IpAddress =>
        _httpContextAccessor.HttpContext?
            .Connection.RemoteIpAddress?.ToString();
}
