using System;
using System.Security.Claims;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;

namespace eCampusGuard.API.Extensions
{
    public static class ClaimsPrincipleExtension
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public static async Task<IEnumerable<AppRole>> GetUserRolesAsync(this ClaimsPrincipal user, IUnitOfWork unitOfWork)
        {
            return (await unitOfWork.AppUsers.GetByIdAsync(GetUserId(user))).UserRoles.Select(x => x.AppRole);
        }
    }
}

