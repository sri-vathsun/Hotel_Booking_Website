
using System.ComponentModel.DataAnnotations;

namespace StayEase.Api.Models
{
    public class RoomType
    {
        public int RoomTypeId { get; set; }

        // Room type name
        [Required]
        [StringLength(100)]
        public string TypeName { get; set; } = string.Empty;

        // Description
        [StringLength(500)]
        public string? Description { get; set; }

        // Navigation property - Rooms
        public ICollection<Room> Rooms { get; set; }
            = new List<Room>();
    }
}
