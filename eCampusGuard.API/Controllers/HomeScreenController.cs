using System;
using eCampusGuard.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eCampusGuard.API.Extensions;
using eCampusGuard.Core.Interfaces;
using eCampusGuard.Core.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using static eCampusGuard.Core.Entities.UserPermit;

namespace eCampusGuard.API.Controllers
{
	[Authorize]
	public class HomeScreenController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;



        public HomeScreenController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet()]
		public async Task<ActionResult<HomeScreenDto>> Home()
		{
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);

            HomeScreenDto result = new HomeScreenDto();

			HashSet<HomeScreenWidget> widgets = new HashSet<HomeScreenWidget>();

			// If the user is NOT a normal user (is admin, gatestaff, or superadmin) then add all the widgets
			if (!roles.Any(r => r == "Member"))
			{
				var rolesEntities = user.UserRoles.Select(ur => ur.Role);
                foreach (AppRole role in rolesEntities)
                {
                    foreach (HomeScreenWidget widget in role.HomeScreenWidgets)
                    {
                        widgets.Add(widget);
                    }
                }
            }
            else // If the user is a normal user then add the relevant widgets
            {
				if (user.UserPermits.Any(up => up.IsPermitValid() || up.Status == UserPermitStatus.Withdrawn))
                {
                    widgets.Add(HomeScreenWidget.PermitStatus);
                    widgets.Add(HomeScreenWidget.AccessLogs);
                } else
                {
                    widgets.Add(HomeScreenWidget.ApplicationStatus);
                    widgets.Add(HomeScreenWidget.PreviousPermits);
                    widgets.Add(HomeScreenWidget.UserApplications);
                }
            }

			result.HomeScreenWidgets = widgets;

			return Ok(result);
		}


		[HttpGet("notifications")]
        public async Task<IEnumerable<NotificationDto>> Notifications()
		{
			var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());
			var notifications = user.Notifications.AsQueryable().ProjectTo<NotificationDto>(_mapper.ConfigurationProvider).AsNoTracking();

            return notifications;
		}
    }
}

