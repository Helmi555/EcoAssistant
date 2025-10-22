using EcoAssistant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoAssistant.Domain.Entities
{
    public class Sensor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LocalId { get; set; }
        public SensorType Type { get; set; } = SensorType.Unknown;
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public bool AlertEnabled { get; set; } = false;
        [Column(TypeName = "jsonb")]
        public string Metadata { get; set; } = "{}";
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public int DeviceId { get; set; }
        public Device Device { get; set; } = null!;
        [NotMapped]
        public string PublicId => $"{DeviceId}-{LocalId}";

    }
}
