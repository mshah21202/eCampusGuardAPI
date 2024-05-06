using System;
namespace eCampusGuard.Core.Entities
{
    public enum UserPermitStatus
    {
        Valid = 0,
        Withdrawn = 1,
        Expired = 2,
    }

    public enum UserPermitOrderBy
    {
        StudentId = 0,
        PlateNumber = 1,
        Status = 2
    }
    public class UserPermit
	{
        public int Id { get; set; }

        public UserPermitStatus Status { get; set; }

		public int UserId { get; set; }
        public virtual AppUser User { get; set; }

		public int PermitId { get; set; }
        public virtual Permit Permit { get; set; }

		public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public int PermitApplicationId { get; set; }
        public virtual PermitApplication PermitApplication { get; set; }

        public virtual IEnumerable<UpdateRequest> UpdateRequests { get; set; }
        public virtual IEnumerable<AccessLog> AccessLogs { get; set; }

        public bool IsPermitValid()
        {
            if (Permit.Expiry < DateTime.Now) return false;

			return true;
        }
    }
}

