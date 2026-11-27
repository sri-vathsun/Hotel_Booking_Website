
using System.ComponentModel.DataAnnotations;

namespace StayEase.Api.Models
{
    public class Facility
    {
        public int FacilityId { get; set; }

        [Required]
        [StringLength(100)]
        public string FacilityName { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Description { get; set; }

        // Navigation property
        public ICollection<HotelFacility> HotelFacilities { get; set; }
            = new List<HotelFacility>();
    }
}
