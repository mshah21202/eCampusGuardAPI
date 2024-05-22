using System;
namespace eCampusGuard.Core.Entities
{
	public class UserNotification
	{
		public int NotificationId { get; set; }
        public virtual Notification Notification { get; set; }
		public int UserId { get; set; }
		public virtual AppUser User { get; set; }
		public bool Read { get; set; }
	}
}

