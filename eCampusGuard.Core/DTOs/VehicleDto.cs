using System;
using System.ComponentModel.DataAnnotations;

namespace eCampusGuard.Core.DTOs
{
	public class VehicleDto
	{
        [Required]
        public string PlateNumber { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required]
        public string Make { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public string Color { get; set; }
        public string? RegistrationDocImgPath { get; set; }
    }
}

