using System;
using eCampusGuard.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
	public class SearchController : BaseApiController
	{
		public SearchController()
		{
		}

		[HttpGet()]
		public async Task<IEnumerable<UserPermitDto>> Search(SearchDto searchDto)
		{
			return new List<UserPermitDto>();
		}
	}
}

