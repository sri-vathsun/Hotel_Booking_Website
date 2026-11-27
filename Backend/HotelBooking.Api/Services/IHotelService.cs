using HotelBooking.Api.DTOs;

namespace HotelBooking.Api.Services;

public interface IHotelService
{
    Task<List<HotelDto>> GetApprovedHotelsAsync();

    Task<HotelDto?> GetHotelByIdAsync(int hotelId);
}