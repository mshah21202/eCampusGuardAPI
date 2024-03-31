using System;
using eCampusGuard.Core.DTOs;

namespace eCampusGuard.Core.Entities
{
	public class UpdateDetailsRequest
	{
		public virtual AppUser User { get; set; }
		public int UserId { get; set; }

		public virtual Permit Permit { get; set; }
		public int PermitId { get; set; }

        public string LicenseImgPath { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}