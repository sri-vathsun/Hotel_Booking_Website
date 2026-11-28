
using StayEase.Api.DTOs.Customer;

namespace StayEase.Api.Services.Interfaces;

public interface ICustomerRoomService
{
    Task<List<RoomAvailabilityDto>> GetAvailableRoomsAsync(
        int hotelId,
        DateTime checkIn,
        DateTime checkOut,
        int guests);
}

