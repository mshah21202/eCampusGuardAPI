using System;
using eCampusGuard.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
	[Authorize]
	public class UserPermitController : BaseApiController
	{
		public UserPermitController()
		{
		}

		[HttpGet()]
		public async Task<List<UserPermitDto>> GetUserPermits()
		{
			return new List<UserPermitDto>();
		}

        [HttpGet("{id}")]
        public async Task<UserPermitDto> GetUserPermit(int id)
		{
			return new UserPermitDto();
		}

		[HttpPost("transfer/{id}")]
		public async Task<ResponseDto> SubmitTransferRequest(TransferRequestDto transferRequestDto)
		{
			return new ResponseDto();
		}

		[HttpPost("details/{id}")]
        public async Task<ResponseDto> SubmitDetailsUpdateRequest(UpdateRequestDto updateRequestDto)
		{
			return new ResponseDto();
		}
    }
}

