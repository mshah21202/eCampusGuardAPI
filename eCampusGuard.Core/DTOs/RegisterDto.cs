using System;
using System.ComponentModel.DataAnnotations;

namespace eCampusGuard.Core.DTOs
{
	public class RegisterDto
	{
		[Required]
		public string Name { get; set; }

		[Required]
		[RegularExpression("^\\d{8}$")]
		public string Username { get; set; }

		[Required]
		[StringLength(32, MinimumLength = 8)]
		public string Password { get; set; }

        [Required]
		[EmailAddress]
		public string Email { get; set; }
    }
}

