using CarChargingApi.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarChargingApi.Models.Requests
{
    public class PatchLocationRequestModel
    {
        public LocationType? Type { get; set; }

        [StringLength(255)]
        public string? Name { get; set; }

        [StringLength(45)]
        public string? Address { get; set; }

        [StringLength(45)]
        public string? City { get; set; }

        [StringLength(10)]
        public string? PostalCode { get; set; }

        [StringLength(45)]
        public string? Country { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
