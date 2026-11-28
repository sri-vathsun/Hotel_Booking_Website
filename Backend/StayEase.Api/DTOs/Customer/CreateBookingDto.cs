
using System.ComponentModel.DataAnnotations;

namespace StayEase.Api.DTOs.Customer;

public class CreateBookingDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    [Required]
    [Range(1, 20)]
    public int NumberOfGuests { get; set; }
}
