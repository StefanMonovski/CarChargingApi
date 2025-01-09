using CarChargingApi.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarChargingApi.Models.Requests
{
    public class LocationRequestModel
    {
        [Required]
        [StringLength(39)]
        public required string LocationId { get; set; }

        [Required]
        public required LocationType Type { get; set; }

        [StringLength(255)]
        public string? Name { get; set; }

        [Required]
        [StringLength(45)]
        public required string Address { get; set; }

        [Required]
        [StringLength(45)]
        public required string City { get; set; }

        [Required]
        [StringLength(10)]
        public required string PostalCode { get; set; }

        [Required]
        [StringLength(45)]
        public required string Country { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }
    }
}
