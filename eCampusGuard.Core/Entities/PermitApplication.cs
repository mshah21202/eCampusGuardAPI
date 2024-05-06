using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace eCampusGuard.Core.Entities
{
	public class PermitApplication
	{
		public enum PermitApplicationStatus
		{
			Pending = 0,
			[Display(Name="Awaiting Payment")]
			AwaitingPayment = 1,
            Denied = 2,
			Paid = 3
		}

		public enum AcademicYear
		{
			[Display(Name="First Year")]
			FirstYear = 0,
			[Display(Name="Second Year")]
            SecondYear = 1,
			[Display(Name="Third Year")]
            ThirdYear = 2,
			[Display(Name="Forth+ Year")]
            FourthPlusYear = 3,
        }

        public enum PermitApplicationOrderBy
        {
			[Display(Name="Student ID")]
            StudentId = 0,
            Name = 1,
			[Display(Name="Academic Year")]
            AcademicYear = 2,
			[Display(Name="Permit Type")]
            PermitType = 3,
			[Display(Name="Status")]
            Status = 4
        }

        public int Id { get; set; }
		public IList<bool> AttendingDays { get; set; }
		public int SiblingsCount { get; set; }
		public AcademicYear Year { get; set; }
		public string LicenseImgPath { get; set; }
        public string PhoneNumberCountry { get; set; }
        public string PhoneNumber { get; set; }
		public PermitApplicationStatus Status { get; set; } = PermitApplicationStatus.Pending;

        public virtual AppUser User { get; set; }
		public int UserId { get; set; }

        public virtual Vehicle Vehicle { get; set; }
		public int VehicleId { get; set; }

        public virtual Permit Permit { get; set; }
		public int PermitId { get; set; }

		public int? UserPermitId { get; set; }
		public virtual UserPermit UserPermit { get; set; }
	}
}

