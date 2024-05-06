using System;
using System.ComponentModel.DataAnnotations;

namespace eCampusGuard.Core.DTOs
{
	public class CreatePermitDto
	{
        [Required]
        public string Name { get; set; }
        [Required]
        public IList<bool> Days { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public int AreaId { get; set; }
        [Required]
        public DateTime Expiry { get; set; }
    }
}

