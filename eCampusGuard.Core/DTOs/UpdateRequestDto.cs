using System;
using eCampusGuard.Core.Entities;

namespace eCampusGuard.Core.DTOs
{
	public class UpdateRequestDto
	{
		public UpdateRequestStatus Status { get; set; }
		public UserPermitDto UserPermit { get; set; }
		public PermitDto NewPermit { get; set; }
		public VehicleDto UpdatedVehicle { get; set; }
	}
}

