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

namespace eCampusGuard.API.Controllers
{
	[Authorize]
	public class HomeScreenController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public HomeScreenController(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		[HttpGet()]
		public async Task<ActionResult<HomeScreenDto>> Home()
		{
			var roles = await User.GetUserRolesAsync(_unitOfWork);
			var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());

			HomeScreenDto result = new HomeScreenDto();

			HashSet<HomeScreenWidget> widgets = new HashSet<HomeScreenWidget>();

			// If the user is NOT a normal user (is admin, gatestaff, or superadmin) then add all the widgets
			if (!roles.Any(r => r.Name == "Member"))
			{
                foreach (AppRole role in roles)
                {
                    foreach (HomeScreenWidget widget in role.HomeScreenWidgets)
                    {
                        widgets.Add(widget);
                    }
                }
            }
            else // If the user is a normal user then add the relevant widgets
            {
				widgets.Add(HomeScreenWidget.PermitStatus);

				var hasPreviousPermits= user.UserPermits.Any();

				if (hasPreviousPermits)
				{
					widgets.Add(HomeScreenWidget.PreviousPermits);
					widgets.Add(HomeScreenWidget.AccessLogs);
                }
                else
				{
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

