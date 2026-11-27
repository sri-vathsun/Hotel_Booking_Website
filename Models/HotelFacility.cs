
namespace StayEase.Api.Models
{
    public class HotelFacility
    {
        // Composite primary key configured in DbContext

        public int HotelId { get; set; }

        public int FacilityId { get; set; }

        // Navigation property - Hotel
        public Hotel Hotel { get; set; } = null!;

        // Navigation property - Facility
        public Facility Facility { get; set; } = null!;
    }
}
