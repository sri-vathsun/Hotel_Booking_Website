
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StayEase.Api.Enums;

namespace StayEase.Api.Models
{
    public class Booking
    {
        // Primary key
        public int BookingId { get; set; }

        // Customer foreign key
        [Required]
        public int CustomerId { get; set; }

        // Hotel foreign key
        [Required]
        public int HotelId { get; set; }

        // Room foreign key
        [Required]
        public int RoomId { get; set; }

        // Booking dates
        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        // Number of guests
        [Required]
        [Range(1, 20)]
        public int NumberOfGuests { get; set; }

        // Total booking amount
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 10000000)]
        public decimal TotalAmount { get; set; }

        // Booking status
        public BookingStatus Status { get; set; }
            = BookingStatus.Pending;

        // Booking creation date
        public DateTime BookedAt { get; set; }
            = DateTime.UtcNow;

        // Navigation property - Customer
        [ForeignKey(nameof(CustomerId))]
        public User Customer { get; set; } = null!;

        // Navigation property - Hotel
        [ForeignKey(nameof(HotelId))]
        public Hotel Hotel { get; set; } = null!;

        // Navigation property - Room
        [ForeignKey(nameof(RoomId))]
        public Room Room { get; set; } = null!;
    }
}
