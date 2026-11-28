
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public interface ICustomerRoomRepository
{
    Task<List<Room>> GetAvailableRoomsAsync(
        int hotelId,
        DateTime checkIn,
        DateTime checkOut,
        int guests);
}
