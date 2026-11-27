
using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using StayEase.Api.Models;
using StayEase.Api.Enums;

namespace StayEase.Api.Repositories;

public class CustomerBookingRepository : ICustomerBookingRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerBookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetRoomAsync(int roomId)
    {
        return await _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.RoomId == roomId);
    }

    public async Task<bool> HasBookingConflictAsync(
        int roomId,
        DateTime checkIn,
        DateTime checkOut)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            (b.Status == BookingStatus.Pending ||
            b.Status == BookingStatus.Confirmed ||
            b.Status == BookingStatus.CheckedIn) &&
            b.CheckInDate < checkOut &&
            b.CheckOutDate > checkIn);
    }

    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return booking;
    }

    public async Task<Booking?> GetBookingAsync(
        int bookingId,
        int customerId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Hotel)
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomType)
            .FirstOrDefaultAsync(b =>
                b.BookingId == bookingId &&
                b.CustomerId == customerId);
    }

    public async Task<List<Booking>> GetCustomerBookingsAsync(
        int customerId)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Hotel)
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomType)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.BookedAt)
            .ToListAsync();
    }

    public async Task<Booking?> GetCustomerBookingForCancellationAsync(
        int bookingId,
        int customerId)
    {
        return await _context.Bookings.FirstOrDefaultAsync(b =>
            b.BookingId == bookingId &&
            b.CustomerId == customerId);
    }

    public async Task UpdateBookingAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }
}
