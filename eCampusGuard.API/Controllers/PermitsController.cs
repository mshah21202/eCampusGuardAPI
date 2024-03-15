using System;
using eCampusGuard.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
	[Authorize]
	public class PermitsController : BaseApiController
	{
		public PermitsController()
		{
		}

		[HttpGet()]
		public async Task<IEnumerable<PermitDto>> GetPermits()
		{
			return new List<PermitDto>();
		}

		[HttpPost()]
		public async Task<ResponseDto> CreatePermit(PermitDto permitDto)
		{
			return new ResponseDto();
		}

		[HttpGet("{id}")]
        public async Task<PermitDto> GetPermit(int id)
		{
			return new PermitDto();
		}

        [HttpPost("{id}")]
        public async Task<ResponseDto> UpdatePermit(int id, PermitDto permitDto)
        {
            return new ResponseDto();
        }

		[HttpDelete("{id}")]
        public async Task<ResponseDto> DeletePermit(int id)
		{
			return new ResponseDto();
		}

    }
}

