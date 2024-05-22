using System;
using System.ComponentModel.DataAnnotations;
using eCampusGuard.Core.Entities;

namespace eCampusGuard.Core.DTOs
{
	public class UpdateRequestDto
	{
<<<<<<< HEAD
        public int  Id { get; set; }
=======
        public int Id { get; set; }
>>>>>>> 37e713337365baa613cf382fce9885407fc08666
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

