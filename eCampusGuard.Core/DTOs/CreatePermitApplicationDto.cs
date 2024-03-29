using System;
using static eCampusGuard.Core.Entities.PermitApplication;

namespace eCampusGuard.Core.DTOs
{
	public class CreatePermitApplicationDto
	{
        public IList<bool> AttendingDays { get; set; }
        public int SiblingsCount { get; set; }
        public string AcademicYear { get; set; }
        public string LicenseImgPath { get; set; }
        public string PhoneNumber { get; set; }
        public VehicleDto Vehicle { get; set; }
        public int PermitId { get; set; }
    }
}

