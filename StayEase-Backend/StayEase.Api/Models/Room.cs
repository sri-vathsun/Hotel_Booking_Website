
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StayEase.Api.Enums;

namespace StayEase.Api.Models
{
    public class Room
    {
        public int RoomId { get; set; }

        // Hotel foreign key
        [Required]
        public int HotelId { get; set; }

        // Room information
        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        // Room type foreign key
        [Required]
        public int RoomTypeId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Capacity
        [Required]
        [Range(1, 20)]
        public int Capacity { get; set; }

        // Price per night
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 1000000)]
        public decimal PricePerNight { get; set; }

        // Room status
        public RoomStatus Status { get; set; }
            = RoomStatus.Available;

        // Navigation property - Hotel
        [ForeignKey(nameof(HotelId))]
        public Hotel Hotel { get; set; } = null!;

        // Navigation property - RoomType
        [ForeignKey(nameof(RoomTypeId))]
        public RoomType RoomType { get; set; } = null!;

        // Navigation property - Bookings
        public ICollection<Booking> Bookings { get; set; }
            = new List<Booking>();
    }
}
