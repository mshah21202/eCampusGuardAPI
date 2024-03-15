using System;
using System.Security.Claims;

namespace eCampusGuard.API.Extensions
{
    public static class ClaimsPrincipleExtension
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}

