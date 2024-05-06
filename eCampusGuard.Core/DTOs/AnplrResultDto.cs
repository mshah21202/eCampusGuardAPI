using System;
using eCampusGuard.Core.Entities;

namespace eCampusGuard.Core.DTOs
{
	public class AnplrResultDto
	{
		public string PlateNumber { get; set; }
		public bool AllowedToEnter { get; set; }
        public string PermitName { get; set; }
        public IList<bool> Days { get; set; }
        public UserPermitStatus Status { get; set; }
    }
}

