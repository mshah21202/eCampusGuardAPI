using System;
namespace eCampusGuard.Core.Entities
{
    public enum AccessLogType
    {
        Entry = 0, Exit = 1
    }

    public class AccessLog
	{
		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public AccessLogType Type { get; set; }

		public int UserPermitId { get; set; }
		public virtual UserPermit UserPermit { get; set; }

		public int AreaId { get; set; }
		public virtual Area Area { get; set; }
	}
}

