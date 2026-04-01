using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;

namespace Infracstructure.Extensions;

public static class ClaimExtension
{
    public static Guid GetCurrentUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (String.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User Id Not found in token");
        }
        if (!Guid.TryParse(userId, out Guid userGuid))
        {
            throw new UnauthorizedAccessException("Invalid User Id in token");
        }
        return userGuid;
    }

    public static string GetCurrentUsername(this ClaimsPrincipal principal)
    {
        var username = principal.FindFirstValue(ClaimTypes.Name);
        if (String.IsNullOrEmpty(username))
        {
            throw new UnauthorizedAccessException("Username Not found in token");
        }
        return username;
    }

    public static string GetCurrentEmail(this ClaimsPrincipal principal)
    {
        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (String.IsNullOrEmpty(email))
        {
            throw new UnauthorizedAccessException("Email Not found in token");
        }
        return email;
    }
}
