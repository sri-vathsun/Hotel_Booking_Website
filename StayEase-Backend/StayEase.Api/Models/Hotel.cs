
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StayEase.Api.Enums;

namespace StayEase.Api.Models
{
    public class Hotel
    {
        public int HotelId { get; set; }

        // Basic information
        [Required]
        [StringLength(150)]
        public string HotelName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        // Address
        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [StringLength(10)]
        public string? PostalCode { get; set; }

        // Contact information
        [Required]
        [Phone]
        [StringLength(15)]
        public string ContactNumber { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        // Hotel rating
        [Range(0, 5)]
        public decimal Rating { get; set; } = 0;

        // Manager foreign key
        [Required]
        public int ManagerId { get; set; }

        [ForeignKey(nameof(ManagerId))]
        public User Manager { get; set; } = null!;

        // Approval status
        public HotelStatus Status { get; set; }
            = HotelStatus.Pending;

        // Navigation properties
        public ICollection<HotelFacility> HotelFacilities { get; set; }
            = new List<HotelFacility>();

        public ICollection<Room> Rooms { get; set; }
            = new List<Room>();
    }
}
