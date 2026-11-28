
using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using StayEase.Api.Enums;
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public class CustomerRoomRepository : ICustomerRoomRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Room>> GetAvailableRoomsAsync(
        int hotelId,
        DateTime checkIn,
        DateTime checkOut,
        int guests)
    {
        return await _context.Rooms
            .AsNoTracking()
            .Include(r => r.RoomType)
            .Where(r =>
                r.HotelId == hotelId &&
                r.Hotel.Status == StayEase.Api.Enums.HotelStatus.Approved &&
                r.Status == StayEase.Api.Enums.RoomStatus.Available &&
                r.Capacity >= guests &&
                !_context.Bookings.Any(b =>
                    b.RoomId == r.RoomId &&
                    b.Status == StayEase.Api.Enums.BookingStatus.Confirmed &&
                    b.CheckInDate < checkOut &&
                    b.CheckOutDate > checkIn))
            .OrderBy(r => r.PricePerNight)
            .ToListAsync();
    }
}

