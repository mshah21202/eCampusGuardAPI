using System;
namespace eCampusGuard.Core.DTOs
{
	public class UpdateUserPermitDto
	{
		public string PhoneNumber { get; set; }
		public string PhoneNumberCountry { get; set; }
		public string? LicenseImgPath { get; set; }
		public int? PermitId { get; set; }
		public VehicleDto Vehicle { get; set; }
	}
}

