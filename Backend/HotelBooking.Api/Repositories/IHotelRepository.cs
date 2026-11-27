using HotelBooking.Api.Models;

namespace HotelBooking.Api.Repositories;

public interface IHotelRepository
{
    Task<List<Hotel>> GetApprovedHotelsAsync();

    Task<Hotel?> GetHotelByIdAsync(int hotelId);
}