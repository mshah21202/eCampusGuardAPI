using System;
using static eCampusGuard.Core.Entities.PermitApplication;

namespace eCampusGuard.Core.DTOs
{
	public class CreatePermitApplicationDto
	{
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public IList<bool> AttendingDays { get; set; }
        public int SiblingsCount { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public string LicenseImgPath { get; set; }
        public string PhoneNumber { get; set; }
        public VehicleDto Vehicle { get; set; }
        public int PermitId { get; set; }
    }
}

