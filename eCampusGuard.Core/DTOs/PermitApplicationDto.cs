using System;
using static eCampusGuard.Core.Entities.PermitApplication;
using System.Collections;

namespace eCampusGuard.Core.DTOs
{
	public class PermitApplicationDto
	{
        public IList<bool> AttendingDays { get; set; }
        public int SiblingsCount { get; set; }
        public string AcademicYear { get; set; }
        public string LicenseImgPath { get; set; }
        public string PhoneNumber { get; set; }
        public PermitApplicationStatusEnum Status { get; set; }
        public VehicleDto Vehicle { get; set; }
        public PermitDto Permit { get; set; }
    }
}

