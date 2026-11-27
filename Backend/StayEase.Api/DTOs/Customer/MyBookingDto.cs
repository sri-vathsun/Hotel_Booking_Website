
using StayEase.Api.Enums;

namespace StayEase.Api.DTOs.Customer;

public class MyBookingDto
{
    public int BookingId { get; set; }

    public string HotelName { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;

    public string RoomType { get; set; } = string.Empty;

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public int NumberOfGuests { get; set; }

    public decimal TotalAmount { get; set; }

    public BookingStatus Status { get; set; }

    public DateTime BookedAt { get; set; }
}
