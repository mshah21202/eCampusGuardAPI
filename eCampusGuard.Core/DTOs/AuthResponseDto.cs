using System;
using System.ComponentModel.DataAnnotations;

namespace eCampusGuard.Core.DTOs
{
	public enum AuthResponseCode
	{
		[Display(Name = "Successfully authenticated")]
		Authenticated = 0,
		[Display(Name = "Successfully registered")]
		RegisteredAndAuthenticated = 1,
		[Display(Name = "Username is already registered")]
		AlreadyRegistered = 2,
		[Display(Name = "Username or password is incorrect")]
        IncorrectCreds = 3,
		[Display(Name = "Something went wrong")]
        Other = 4
	}

	public class AuthResponseDto
	{
		public AuthResponseCode Code { get; set; }
		public string Token { get; set; }
		public object Error { get; set; }
	}
}

