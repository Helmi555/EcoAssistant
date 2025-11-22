using EcoAssistant.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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


        public string PictureUrl { get; set; } = string.Empty;

        public string DeviceName { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;


        public Guid? GroupId { get; set; }
        // added: navigation back to group
        [JsonIgnore]
        public Group? Group { get; set; }


        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
        

    }
}
