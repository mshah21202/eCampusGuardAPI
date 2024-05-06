using System;
using static eCampusGuard.Core.Entities.PermitApplication;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace eCampusGuard.Core.DTOs
{
	public class PermitApplicationDto
	{
        public int? StudentId { get; set; }
        public string? StudentName { get; set; }
        public IList<bool> AttendingDays { get; set; }
        public int SiblingsCount { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public string? LicenseImgPath { get; set; }
        public string PhoneNumberCountry { get; set; }
        public string PhoneNumber { get; set; }
        public PermitApplicationStatus Status { get; set; }
        public VehicleDto Vehicle { get; set; }
        public PermitDto Permit { get; set; }
        public int? UserPermitId { get; set; }
    }
}

