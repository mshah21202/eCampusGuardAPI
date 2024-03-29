using System;
using static eCampusGuard.Core.Entities.AccessLog;

namespace eCampusGuard.Core.DTOs
{
	public class AccessLogDto
	{
		public DateTime Timestamp { get; set; }
		public string LicensePlate { get; set; }
		public AccessLogType LogType { get; set; }
	}
}

