using System;
namespace eCampusGuard.Core.Entities
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
        public string Password { get; set; } // Convert to hash?
		public string Role { get; set; } // Use RoleManager?
        public virtual IEnumerable<Vehicle> Vehicles { get; set; }
        public virtual IEnumerable<PermitApplication> PermitApplications { get; set; }
        public virtual IEnumerable<AccessLog> AccessLogs { get; set; }
        public virtual IEnumerable<UserPermit> UserPermits { get; set; }

	}
}

