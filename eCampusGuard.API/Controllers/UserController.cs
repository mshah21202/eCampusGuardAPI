using System;
using eCampusGuard.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
	[Authorize]
	public class UserController : BaseApiController
	{
		public UserController()
		{
		}

		[HttpGet()]
		public async Task<HomeScreenDto> Home()
		{
			return new HomeScreenDto();
		}


		[HttpGet("notifications")]
        public async Task<IEnumerable<NotificationDto>> Notifications()
		{
			return new List<NotificationDto>();
		}
    }
}

