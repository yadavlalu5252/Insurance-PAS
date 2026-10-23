using Microsoft.AspNetCore.Http;

namespace SupportAudit.Api.Infrastructure;

public sealed class RequestUserContext
{
    private readonly IHttpContextAccessor _accessor;

    public RequestUserContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string UserId =>
        _accessor.HttpContext?
            .Request.Headers["X-User-Id"]
            .FirstOrDefault()
        ?? string.Empty;

    public string Role =>
        _accessor.HttpContext?
            .Request.Headers["X-User-Role"]
            .FirstOrDefault()
        ?? string.Empty;

    public string? IpAddress =>
        _accessor.HttpContext?
            .Connection.RemoteIpAddress?
            .ToString();
}