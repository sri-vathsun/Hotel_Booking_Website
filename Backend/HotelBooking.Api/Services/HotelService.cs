using HotelBooking.Api.DTOs;
using HotelBooking.Api.Repositories;

namespace HotelBooking.Api.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _repository;

    public HotelService(IHotelRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<HotelDto>> GetApprovedHotelsAsync()
    {
        var hotels = await _repository.GetApprovedHotelsAsync();

        return hotels.Select(h => new HotelDto
        {
            HotelId = h.HotelId,
            Name = h.Name,
            Description = h.Description,
            Address = h.Address,
            City = h.City,
            State = h.State,
            Country = h.Country,
            StarRating = h.StarRating
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
            Name = hotel.Name,
            Description = hotel.Description,
            Address = hotel.Address,
            City = hotel.City,
            State = hotel.State,
            Country = hotel.Country,
            StarRating = hotel.StarRating
        };
    }
}