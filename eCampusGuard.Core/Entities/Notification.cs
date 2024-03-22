using System;
namespace eCampusGuard.Core.Entities
{
	public class Notification
	{
		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }
		public int UserId { get; set; }
		public virtual AppUser User { get; set; }
	}
}

