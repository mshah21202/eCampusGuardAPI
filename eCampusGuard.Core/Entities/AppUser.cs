using System;
using Microsoft.AspNetCore.Identity;

namespace eCampusGuard.Core.Entities
{
	public class AppUser : IdentityUser<int>
	{
		public string Name { get; set; }
		public virtual ICollection<AppRole> Roles { get; set; } // Use RoleManager?
        public virtual IEnumerable<Vehicle> Vehicles { get; set; }
        public virtual IEnumerable<PermitApplication> PermitApplications { get; set; }
        public virtual IEnumerable<AccessLog> AccessLogs { get; set; }
        public virtual IEnumerable<UserPermit> UserPermits { get; set; }

	}
}

