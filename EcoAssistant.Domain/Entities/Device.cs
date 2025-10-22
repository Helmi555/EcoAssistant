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
    public class Device
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DeviceStatus Status { get; set; } = DeviceStatus.Unknown;

        [Column(TypeName = "jsonb")]
        public string Metadata { get; set; } = "{}";

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
    }
}
