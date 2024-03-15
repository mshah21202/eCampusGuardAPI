using System;
using System.Security.Cryptography;
using eCampusGuard.Core.DTOs;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using eCampusGuard.MSSQL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eCampusGuard.API.Controllers
{
	public class AuthenticationController : BaseApiController
	{
		private readonly SQLDataContext _context;
		private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenService _tokenService;


        public AuthenticationController(SQLDataContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
		{
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenService = tokenService;
        }

		[HttpPost("login")]
		public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            // Check if user exists

            if (!await UserExists(loginDto.Username)) return NotFound(new AuthResponseDto
            {
                Code = AuthResponseCode.IncorrectCreds
            });

            // Try to authenticate

            var user = await _context.AppUsers.FirstAsync(x => x.UserName == loginDto.Username);

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (result.Succeeded) return Ok(new AuthResponseDto
            {
                Code = AuthResponseCode.Authenticated,
                Token = await _tokenService.CreateToken(user)
            });

            return BadRequest(new AuthResponseDto
            {
                Code = AuthResponseCode.IncorrectCreds,
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            // If user already exists, return error
            if (await UserExists(registerDto.Username))
            {
                return new AuthResponseDto
                {
                    Code = AuthResponseCode.AlreadyRegistered
                };
            }

            // Create user

            var user = new AppUser
            {
                UserName = registerDto.Username,
                Name = registerDto.Name
            };


            var result = await _userManager.CreateAsync(user, registerDto.Password);

            // If nothing fails, return token
            if (result.Succeeded)

                return Ok(new AuthResponseDto
                {
                    Code = AuthResponseCode.RegisteredAndAuthenticated,
                    Token = await _tokenService.CreateToken(user)
                });


            return BadRequest(new AuthResponseDto
            {
                Code = AuthResponseCode.Other
            });
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.AppUsers.AnyAsync(x => x.UserName == username);
        }
    }
}

