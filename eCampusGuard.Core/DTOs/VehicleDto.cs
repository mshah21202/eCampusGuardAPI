using System;
namespace eCampusGuard.Core.DTOs
{
	public class VehicleDto
	{
        public string PlateNumber { get; set; }
        public string Nationality { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public string? RegistrationDocImgPath { get; set; }
    }
}

