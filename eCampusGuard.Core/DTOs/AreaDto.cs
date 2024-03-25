using System;
namespace eCampusGuard.Core.DTOs
{
	public class AreaDto
	{
        public string Name { get; set; }
        public string Gate { get; set; }
        public int Occupied { get; set; }
        public int Capacity { get; set; }
    }
}

