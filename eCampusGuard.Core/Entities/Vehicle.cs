using System;
namespace eCampusGuard.Core.Entities
{
	public class Vehicle
	{
		public int Id { get; set; }
		public string PlateNumber { get; set; }
		public string Nationality { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }
		public int Year { get; set; }
		public string Color { get; set; }
		public string RegistrationDocImgPath { get; set; }

		public int UserId { get; set; }
        public virtual AppUser User { get; set; }

        public virtual IEnumerable<AccessLog> AccessLogs { get; set; }
        public virtual IEnumerable<UserPermit> UserPermits { get; set; }

    }
}

