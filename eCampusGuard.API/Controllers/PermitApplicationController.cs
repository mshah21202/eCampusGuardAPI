using System;
using eCampusGuard.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
    [Authorize]
	public class PermitApplicationController : BaseApiController
	{
		public PermitApplicationController()
		{
		}

        [HttpGet()]
        public async Task<IEnumerable<PermitApplicationDto>> GetPermitApplications()
        {
            return new List<PermitApplicationDto>();
        }

        [HttpGet("{id}")]
        public async Task<PermitApplicationDto> GetPermitApplication(int id)
        {
            return new PermitApplicationDto();
        }

        [HttpPost("apply")]
        public async Task<ResponseDto> Apply(PermitApplicationDto permitApplicationDto)
        {
            return new ResponseDto();
        }

        [HttpPost("response/{id}")]
        public async Task<ResponseDto> SubmitApplicationResponse(PermitApplicationDto permitApplicationDto)
        {
            return new ResponseDto();
        }
    }
}

