using System;
namespace eCampusGuard.Core.Entities
{
	public class Area
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Gate { get; set; }
		public int Occupied { get; set; }
		public int Capacity { get; set; }
        public virtual IEnumerable<Permit> Permits { get; set; }
	}
}

