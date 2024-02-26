using System;
namespace eCampusGuard.Core.Entities
{
	public class UserPermit
	{
		public int Status { get; set; }
		public DateTime Expiry { get; set; }

		public int UserId { get; set; }
        public virtual User User { get; set; }

		public int PermitId { get; set; }
        public virtual Permit Permit { get; set; }

		public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }
	}
}

