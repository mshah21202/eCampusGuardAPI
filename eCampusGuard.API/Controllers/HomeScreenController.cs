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



        public HomeScreenController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
            _userManager = userManager;
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
                var userPermits = await _unitOfWork.UserPermits.FindAllAsync(up => up.UserId == user.Id);
                var applications = await _unitOfWork.PermitApplications.FindAllAsync(pa => pa.UserId == user.Id);

                
				if (userPermits.Count() > 0 && !applications.Any(a => a.Status == PermitApplication.PermitApplicationStatus.Pending || a.Status == PermitApplication.PermitApplicationStatus.AwaitingPayment))
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
        public async Task<ActionResult<IEnumerable<NotificationDto>>> Notifications()
		{
			var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());
            if (user == null)
            {
                return NotFound("User not found");
            }
            var notifications = user.UserNotifications.OrderByDescending(un => un.Notification.Timestamp).AsQueryable().ProjectTo<NotificationDto>(_mapper.ConfigurationProvider).AsNoTracking();

            return Ok(notifications);
		}

        [HttpPost("notifications/{id}")]
        public async Task<ActionResult> ReadNotification(int id)
        {
            var user = await _unitOfWork.AppUsers.GetByIdAsync(User.GetUserId());
            if (user == null)
            {
                return NotFound("User not found");
            }

            var usernotification = await _unitOfWork.UserNotifications.FindAsync(un => un.UserId == User.GetUserId() && un.NotificationId == id);
            if (usernotification == null)
            {
                return NotFound("Notification not found");
            }

            usernotification.Read = true;

            _unitOfWork.UserNotifications.Update(usernotification);

            if (await _unitOfWork.CompleteAsync() < 1)
            {
                return BadRequest("Something went wrong");
            } 

            return Ok();
        }
    }
}

