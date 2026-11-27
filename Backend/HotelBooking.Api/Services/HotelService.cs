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
}