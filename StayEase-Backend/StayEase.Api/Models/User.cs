using System.ComponentModel.DataAnnotations;
using StayEase.Api.Enums;

namespace StayEase.Api.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Hotels managed by this user
        public ICollection<Hotel> ManagedHotels { get; set; }
            = new List<Hotel>();

        // Bookings made by this user
        public ICollection<Booking> CustomerBookings { get; set; }
            = new List<Booking>();
    }
}
