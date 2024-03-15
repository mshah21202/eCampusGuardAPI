using System;
using Microsoft.AspNetCore.Identity;

namespace eCampusGuard.Core.Entities
{
	public class AppRole : IdentityRole<int>
	{
		public virtual IEnumerable<AppUser> AppUsers { get; set; }

    }
}

