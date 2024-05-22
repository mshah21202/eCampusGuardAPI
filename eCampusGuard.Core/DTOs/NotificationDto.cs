using System;
namespace eCampusGuard.Core.DTOs
{
	public class NotificationDto
	{
		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }
		public bool Read { get; set; }
	}
}

