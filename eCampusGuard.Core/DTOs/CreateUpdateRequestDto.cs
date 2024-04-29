using System;
namespace eCampusGuard.Core.DTOs
{
	public class CreateUpdateRequestDto
	{
        public VehicleDto Vehicle { get; set; }
        public int PermitId { get; set; }
    }
}

