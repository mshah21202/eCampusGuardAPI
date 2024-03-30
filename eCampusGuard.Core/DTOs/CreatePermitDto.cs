using System;
namespace eCampusGuard.Core.DTOs
{
	public class CreatePermitDto
	{
        public string Name { get; set; }
        public IList<bool> Days { get; set; }
        public float Price { get; set; }
        public int Capacity { get; set; }
        public int AreaId { get; set; }
    }
}

