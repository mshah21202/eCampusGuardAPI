using System;
using System.Collections;
using eCampusGuard.Core.Entities;

namespace eCampusGuard.Core.DTOs
{
	public class PermitDto
	{
        public string Name { get; set; }
        public BitArray Days { get; set; }
        public float Price { get; set; }
        public int Occupied { get; set; }
        public int Capacity { get; set; }
        public AreaDto Area { get; set; }

    }
}

