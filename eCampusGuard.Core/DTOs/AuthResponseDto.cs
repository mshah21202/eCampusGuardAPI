using System;
namespace eCampusGuard.Core.DTOs
{
	public enum AuthResponseCode
	{
		Authenticated = 0,
		RegisteredAndAuthenticated = 1,
		AlreadyRegistered = 2,
		IncorrectCreds = 3,
		Other = 4
	}

	public class AuthResponseDto
	{
		public AuthResponseCode Code { get; set; }
		public string Token { get; set; }
		public object Error { get; set; }
	}
}

