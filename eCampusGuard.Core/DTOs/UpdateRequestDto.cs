using System;
namespace eCampusGuard.Core.DTOs
{
	public class UpdateRequestDto
	{
        public string LicenseImgPath { get; set; }
        public VehicleDto Vehicle { get; set; }
    }
}

