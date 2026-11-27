
using StayEase.Api.DTOs.Customer;
using StayEase.Api.Repositories;
using StayEase.Api.Services.Interfaces;

namespace StayEase.Api.Services.Implementations;

public class CustomerHotelService : ICustomerHotelService
{
    private readonly ICustomerHotelRepository _repository;

    public CustomerHotelService(ICustomerHotelRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<HotelDto>> GetApprovedHotelsAsync(
        string? search,
        string? city,
        int? minStarRating)
    {
        var hotels = await _repository.GetApprovedHotelsAsync(
            search, city, minStarRating);

        return hotels.Select(h => new HotelDto
        {
            HotelId = h.HotelId,
            Name = h.HotelName,
            Description = h.Description ?? string.Empty,
            Address = h.Address,
            City = h.City,
            State = h.State,
            Country = h.Country,
            StarRating = (int)Math.Round(h.Rating)
        }).ToList();
    }

    public async Task<HotelDto?> GetHotelByIdAsync(int hotelId)
    {
        var hotel = await _repository.GetHotelByIdAsync(hotelId);

        if (hotel == null)
        {
            return null;
        }

        return new HotelDto
        {
            HotelId = hotel.HotelId,
            Name = hotel.HotelName,
            Description = hotel.Description ?? string.Empty,
            Address = hotel.Address,
            City = hotel.City,
            State = hotel.State,
            Country = hotel.Country,
            StarRating = (int)Math.Round(hotel.Rating)
        };
    }
}
