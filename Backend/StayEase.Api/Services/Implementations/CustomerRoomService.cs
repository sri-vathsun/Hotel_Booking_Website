
using StayEase.Api.DTOs.Customer;
using StayEase.Api.Repositories;
using StayEase.Api.Services.Interfaces;

namespace StayEase.Api.Services.Implementations;

public class CustomerRoomService : ICustomerRoomService
{
    private readonly ICustomerRoomRepository _repository;

    public CustomerRoomService(ICustomerRoomRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RoomAvailabilityDto>> GetAvailableRoomsAsync(
        int hotelId,
        DateTime checkIn,
        DateTime checkOut,
        int guests)
    {
        var rooms = await _repository.GetAvailableRoomsAsync(
            hotelId, checkIn, checkOut, guests);

        return rooms.Select(r => new RoomAvailabilityDto
        {
            RoomId = r.RoomId,
            HotelId = r.HotelId,
            RoomNumber = r.RoomNumber,
            RoomType = r.RoomType.TypeName,
            Capacity = r.Capacity,
            PricePerNight = r.PricePerNight,
            Description = r.Description
        }).ToList();
    }
}
