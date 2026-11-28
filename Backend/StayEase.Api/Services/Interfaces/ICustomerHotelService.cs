
using StayEase.Api.DTOs.Customer;

namespace StayEase.Api.Services.Interfaces;

public interface ICustomerHotelService
{
    Task<List<HotelDto>> GetApprovedHotelsAsync(
        string? search,
        string? city,
        int? minStarRating);

    Task<HotelDto?> GetHotelByIdAsync(int hotelId);
}
