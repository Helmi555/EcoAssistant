using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoAssistant.Domain.Entities
{
    public class Mesure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public double Value { get; set; }

        public bool AlertedMesure { get; set; } = false;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public int? SensorLocalId { get; set; }
        public int? SensorDeviceId { get; set; }
        public Sensor? Sensor { get; set; }
    }
}
