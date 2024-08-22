using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace VietGeeks.TestPlatform.AspNetCore.Services;

public class Tenant(IHttpContextAccessor httpContextAccessor) : ITenant
{
    private IEnumerable<Claim> _claims => httpContextAccessor?.HttpContext?.User?.Claims ?? Array.Empty<Claim>();
    public string Email => GetClaim(ClaimTypes.Email);

    public string UserId => GetClaim(ClaimTypes.NameIdentifier);

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