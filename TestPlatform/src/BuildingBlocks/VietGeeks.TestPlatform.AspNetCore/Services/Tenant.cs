
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace VietGeeks.TestPlatform.AspNetCore;

public class Tenant : ITenant
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Tenant(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string Email => GetClaim(ClaimTypes.Email);

    public string UserId => GetClaim(ClaimTypes.NameIdentifier);

    private IEnumerable<Claim> _claims => _httpContextAccessor?.HttpContext?.User?.Claims ?? Array.Empty<Claim>();

    private string GetClaim(string claimType)
    {
        var claim = _claims.FirstOrDefault(c => c.Type == claimType);
        return claim?.Value ?? "";
    }

    private string GetUserId()
    {
        var claim = GetClaim(ClaimTypes.NameIdentifier);
        return claim.Replace("auth0|", string.Empty);
    }
}

