using System;
using System.ComponentModel.DataAnnotations;

namespace eCampusGuard.Core.DTOs
{
	public class CreateUpdateRequestDto
	{
        [Required]
        public VehicleDto Vehicle { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? PhoneNumberCountry { get; set; }
        public string? DrivingLicenseImgPath { get; set; }
    }
}

