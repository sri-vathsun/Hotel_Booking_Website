using HotelBooking.Api.Models;

namespace HotelBooking.Api.Repositories;

public interface IHotelRepository
{
    Task<List<Hotel>> GetApprovedHotelsAsync(
        string? search,
        string? city,
        int? minStarRating);

    Task<Hotel?> GetHotelByIdAsync(int hotelId);
}