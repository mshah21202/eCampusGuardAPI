using System;
using eCampusGuard.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace eCampusGuard.API.Controllers
{
	public class AuthenticationController : BaseApiController
	{
		public AuthenticationController()
		{
		}

		[HttpPost("login")]
		public async Task<AuthResponseDto> Login(LoginDto loginDto)
		{
			return new AuthResponseDto();
		}

        [HttpPost("register")]
        public async Task<AuthResponseDto> Register(RegisterDto registerDto)
        {
            return new AuthResponseDto();
        }
    }
}

