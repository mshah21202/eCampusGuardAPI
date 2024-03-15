using System;
using Microsoft.AspNetCore.Identity;

namespace eCampusGuard.Core.Entities
{
	public class AppUserRole : IdentityUserRole<int>
	{
		public virtual AppUser AppUser { get; set; }
		public virtual AppRole AppRole { get; set; }
	}
}

