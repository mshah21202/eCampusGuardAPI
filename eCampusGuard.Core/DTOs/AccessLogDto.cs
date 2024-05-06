using System;
using eCampusGuard.Core.Entities;

namespace eCampusGuard.Core.DTOs
{
	public class AccessLogDto
	{
		public DateTime Timestamp { get; set; }
		public string LicensePlate { get; set; }
		public string PermitName { get; set; }
        public AccessLogType LogType { get; set; }
	}
}

