﻿using System;
namespace eCampusGuard.Core.Entities
{
	public class PermitApplication
	{
		public int Id { get; set; }
		public int AttendingDays { get; set; }
		public int SiblingsCount { get; set; }
		public int AcademicYear { get; set; }
		public string LicenseImgPath { get; set; }
		public string PhoneNumber { get; set; }
		public int Status { get; set; } // Change to enum

		public int UserId { get; set; }
        public virtual User User { get; set; }

		public int VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

		public int PermitId { get; set; }
        public virtual Permit Permit { get; set; }
	}
}

