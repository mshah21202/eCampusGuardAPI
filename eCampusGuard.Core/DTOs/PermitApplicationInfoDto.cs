using System;
using static eCampusGuard.Core.Entities.PermitApplication;

namespace eCampusGuard.Core.DTOs
{
	public class PermitApplicationInfoDto
	{
		public int Id { get; set; }
		public string PermitName { get; set; }
		public string AcademicYear { get; set; }
		public PermitApplicationStatusEnum Status { get; set; }
	}
}

