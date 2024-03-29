using System;
using System.Collections;

namespace eCampusGuard.Core.Entities
{
	public class PermitApplication
	{
		public enum PermitApplicationStatusEnum
		{
			Pending = 0,
			AwaitingPayment = 1,
			Denied = 2,
			Paid = 3
		}

		public int Id { get; set; }
		public IList<bool> AttendingDays { get; set; }
		public int SiblingsCount { get; set; }
		public string AcademicYear { get; set; }
		public string LicenseImgPath { get; set; }
		public string PhoneNumber { get; set; }
		public PermitApplicationStatusEnum Status { get; set; } = PermitApplicationStatusEnum.Pending;

		public int UserId { get; set; }
        public virtual AppUser User { get; set; }

		public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

		public int PermitId { get; set; }
        public virtual Permit Permit { get; set; }
	}
}

