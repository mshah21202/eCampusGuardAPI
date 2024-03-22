using System;
namespace eCampusGuard.Core.Entities
{
	public class UserPermit
	{
		public enum UserPermitStatus
		{
			Valid = 0,
			Withdrawn = 1,
			Expired = 2,
		}

		public UserPermitStatus Status { get; set; }
		public DateTime Expiry { get; set; }

		public int UserId { get; set; }
        public virtual AppUser User { get; set; }

		public int PermitId { get; set; }
        public virtual Permit Permit { get; set; }

		public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public bool IsPermitValid()
        {
            if (Expiry < DateTime.Now) return false;

			return true;
        }
    }
}

