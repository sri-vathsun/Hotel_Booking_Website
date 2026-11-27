
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public interface ICustomerHotelRepository
{
    Task<List<Hotel>> GetApprovedHotelsAsync(
        string? search,
        string? city,
        int? minStarRating);

    Task<Hotel?> GetHotelByIdAsync(int hotelId);
}
