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

        //public static async Task<IList<AppRole>> GetUserRolesAsync(this ClaimsPrincipal user, IUnitOfWork unitOfWork)
        //{
        //    var id = GetUserId(user);
        //    var userr = await unitOfWork.AppUsers.GetByIdAsync(id);
        //    return userr.UserRoles.Select(x => x.Role).ToList();
        //}
    }
}

