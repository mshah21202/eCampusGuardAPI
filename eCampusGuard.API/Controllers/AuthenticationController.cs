using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using eCampusGuard.Core.DTOs;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using eCampusGuard.MSSQL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace eCampusGuard.API.Controllers
{
	public class AuthenticationController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenService _tokenService;


        public AuthenticationController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenService = tokenService;
        }

		[HttpPost("login")]
		public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            // Try to authenticate
            try
            {
                var user = await _unitOfWork.AppUsers.FindAsync(u => u.UserName == loginDto.Username);

                if (user == null)
                {
                    return NotFound();
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);


                if (result.Succeeded) return Ok(new AuthResponseDto
                {
                    Code = AuthResponseCode.Authenticated,
                    Token = await _tokenService.CreateToken(user)
                });
            }
            catch (Exception e)
            {
                return Ok(new AuthResponseDto
                {
                    Code = AuthResponseCode.Other,
                    Error = e.Message
                });
            }
            

            return Ok(new AuthResponseDto
            {
                Code = AuthResponseCode.IncorrectCreds,
                Error = AuthResponseCode.IncorrectCreds.GetAttributeOfType<DisplayAttribute>().Name
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            // If user already exists, return error
            if (await UserExists(registerDto.Username))
            {
                return Ok(new AuthResponseDto
                {
                    Code = AuthResponseCode.AlreadyRegistered,
                    Error = AuthResponseCode.AlreadyRegistered.GetAttributeOfType<DisplayAttribute>().Name
                });
            }

            try
            {
                // Create user
                var user = new AppUser
                {
                    UserName = registerDto.Username,
                    Name = registerDto.Name
                };
                var result = await _userManager.CreateAsync(user, registerDto.Password);

              // If nothing fails, return token
              if (result.Succeeded)
              {
                  var roleResult = await _userManager.AddToRoleAsync(user, "Member");
                  if (roleResult.Succeeded)
                      return Ok(new AuthResponseDto
                      {
                          Code = AuthResponseCode.RegisteredAndAuthenticated,
                          Token = await _tokenService.CreateToken(user)
                      });
              }
            }
            catch (Exception e)
            {
                return Ok(new AuthResponseDto
                {
                    Code = AuthResponseCode.Other,
                    Error = e.Message
                });
            }

            return Ok(new AuthResponseDto
            {
                Code = AuthResponseCode.Other,
                Error = AuthResponseCode.Other.GetAttributeOfType<DisplayAttribute>().Name

            });
        }

        private async Task<bool> UserExists(string username)
        {
            return (await _unitOfWork.AppUsers.FindAsync(x => x.UserName == username)) != null;
        }
    }
}

