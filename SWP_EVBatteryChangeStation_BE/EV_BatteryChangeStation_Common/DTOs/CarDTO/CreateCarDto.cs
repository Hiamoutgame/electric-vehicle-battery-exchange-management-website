using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_BatteryChangeStation_Common.DTOs.CarDTO
{
    public class CreateCarDto
    {
        public Guid? OwnerId { get; set; }
        public string? Vin { get; set; }
        public string? LicensePlate { get; set; }
        public string Model { get; set; } = string.Empty;
        public string BatteryType { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;

        public DateTime? CreateDate { get; set; } = DateTime.UtcNow;

        public string? Images { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }
}
