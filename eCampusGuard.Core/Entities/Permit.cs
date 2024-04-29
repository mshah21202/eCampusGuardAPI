using System;
using System.Collections;

namespace eCampusGuard.Core.Entities
{
	public class Permit
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public IList<bool> Days { get; set; }
		public float Price { get; set; }
		public int Occupied { get; set; }
		public int Capacity { get; set; }
		public int AreaId { get; set; }
        public virtual Area Area { get; set; }
        public virtual IEnumerable<UserPermit> UserPermits { get; set; }
        public virtual IEnumerable<UpdateRequest> UpdateRequests { get; set; }

    }
}

