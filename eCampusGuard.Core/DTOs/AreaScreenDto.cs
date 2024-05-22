using System;
namespace eCampusGuard.Core.DTOs
{
	public class AreaScreenDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Gate { get; set; }
        public IEnumerable<AccessLogDto> AccessLogs { get; set; }
		public int Occupied { get; set; }
		public int Capacity { get; set; }
    }
}

