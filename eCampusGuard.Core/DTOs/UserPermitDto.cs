using System;
using eCampusGuard.Core.Entities;

namespace eCampusGuard.Core.DTOs
{
	public class UserPermitDto
	{
		public UserPermitStatus Status { get; set; }
		public DateTime Expiry { get; set; }
		public UserDto User { get; set; }
		public PermitDto Permit { get; set; }
		public VehicleDto Vehicle { get; set; }
		public IEnumerable<AccessLogDto> AccessLogs { get; set; }
	}
}

