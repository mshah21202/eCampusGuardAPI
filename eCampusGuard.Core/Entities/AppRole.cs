using System;
using Microsoft.AspNetCore.Identity;

namespace eCampusGuard.Core.Entities
{
	public class AppRole : IdentityRole<int>
	{
		public virtual IEnumerable<AppUserRole> UserRoles { get; set; }
        public IEnumerable<HomeScreenWidget> HomeScreenWidgets { get; set; }
	}
}

