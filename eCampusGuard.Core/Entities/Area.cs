using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCampusGuard.Core.Entities
{
	public class Area
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Gate { get; set; }
		public int Occupied { get; set; } = 0;
		public int CurrentOccupied { get; set; } = 0;
        public int Capacity { get; set; }
        public virtual IEnumerable<Permit> Permits { get; set; }
		public virtual IEnumerable<AccessLog> AccessLogs { get; set; }
	}
}

