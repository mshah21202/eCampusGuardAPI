﻿using System;
using Microsoft.AspNetCore.Identity;

namespace eCampusGuard.Core.Entities
{
	public class AppUser : IdentityUser<int>
	{
		public string Name { get; set; }
		public virtual ICollection<AppUserRole> UserRoles { get; set; }
        public virtual IEnumerable<Vehicle> Vehicles { get; set; }
        public virtual IEnumerable<PermitApplication> PermitApplications { get; set; }
        public virtual IEnumerable<UserPermit> UserPermits { get; set; }
        public virtual IEnumerable<UserNotification> UserNotifications { get; set; }

    }
}

