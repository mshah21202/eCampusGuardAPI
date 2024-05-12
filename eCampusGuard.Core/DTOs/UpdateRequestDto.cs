using System;
using System.ComponentModel.DataAnnotations;
using eCampusGuard.Core.Entities;

namespace eCampusGuard.Core.DTOs
{
	public class UpdateRequestDto
	{
        public int Id { get; set; }
        public UpdateRequestStatus Status { get; set; }
		public UserPermitDto UserPermit { get; set; }
		public VehicleDto UpdatedVehicle { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? PhoneNumberCountry { get; set; }
        public string? DrivingLicenseImgPath { get; set; }
    }
}

