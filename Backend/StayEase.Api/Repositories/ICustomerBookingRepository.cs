
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public interface ICustomerBookingRepository
{
    Task<Room?> GetRoomAsync(int roomId);

    Task<bool> HasBookingConflictAsync(
        int roomId,
        DateTime checkIn,
        DateTime checkOut);

    Task<Booking> CreateBookingAsync(Booking booking);

    Task<Booking?> GetBookingAsync(
        int bookingId,
        int customerId);

    Task<List<Booking>> GetCustomerBookingsAsync(int customerId);

    Task<Booking?> GetCustomerBookingForCancellationAsync(
        int bookingId,
        int customerId);

    Task UpdateBookingAsync(Booking booking);
}
