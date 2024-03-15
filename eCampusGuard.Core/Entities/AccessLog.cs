using System;
namespace eCampusGuard.Core.Entities
{
	public class AccessLog
	{
		public enum AccessLogType
		{
			Entry = 0, Exit = 1
		}

		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public AccessLogType Type { get; set; }
		
		public int VehicleId { get; set; }
		public virtual Vehicle Vehicle { get; set; }

		public int PermitId { get; set; }
        public virtual Permit Permit { get; set; }

		public int UserId { get; set; }
        public virtual AppUser User { get; set; }
	}
}

