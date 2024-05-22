using System;
using System.ComponentModel.DataAnnotations;
using static eCampusGuard.Core.Entities.PermitApplication;

namespace eCampusGuard.Core.DTOs
{
	public class CreatePermitApplicationDto
	{
        [Required]
        public int StudentId { get; set; }
        [Required]
        public IList<bool> AttendingDays { get; set; }
        [Required]
        public int SiblingsCount { get; set; }
        [Required]
        public AcademicYear AcademicYear { get; set; }
        [Required]
        public string LicenseImgPath { get; set; }
        [Required]
        public string PhoneNumberCountry { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public VehicleDto Vehicle { get; set; }
        [Required]
        public int PermitId { get; set; }
    }
}

