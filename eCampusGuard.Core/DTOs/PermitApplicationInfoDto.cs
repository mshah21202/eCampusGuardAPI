using System;
using static eCampusGuard.Core.Entities.PermitApplication;

namespace eCampusGuard.Core.DTOs
{
	public class PermitApplicationInfoDto
	{
		public int Id { get; set; }
		public int StudentId { get; set; }
		public string StudentName { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public string PermitName { get; set; }
		public PermitApplicationStatus Status { get; set; }
	}
}

