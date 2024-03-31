using System;
namespace eCampusGuard.Core.Entities
{
	public class TransferRequest
	{
        public virtual AppUser FromUser { get; set; }
		public int FromUserId { get; set; }

        public virtual AppUser ToUser { get; set; }
        public int ToUserId { get; set; }

        public virtual Permit Permit { get; set; }
        public int PermitId { get; set; }
    }
}

