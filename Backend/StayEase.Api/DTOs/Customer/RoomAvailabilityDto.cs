
namespace StayEase.Api.DTOs.Customer;

public class RoomAvailabilityDto
{
    public int RoomId { get; set; }

    public int HotelId { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public string RoomType { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public decimal PricePerNight { get; set; }

    public string? Description { get; set; }
}
