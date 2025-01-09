using CarChargingApi.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CarChargingApi.Models.Data
{
    public class ChargePoint
    {
        [Key]
        [StringLength(39)]
        public required string ChargePointId { get; set; }

        [Required]
        public required ChargePointStatus Status { get; set; }

        [StringLength(4)]
        public string? FloorLevel { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        // Navigation properties

        [JsonIgnore]
        [StringLength(39)]
        public string? LocationId { get; set; }

        [JsonIgnore]
        [ForeignKey("LocationId")]
        public Location? Location { get; set; }
    }
}
