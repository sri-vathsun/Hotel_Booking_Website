
using StayEase.Api.DTOs.Customer;

namespace StayEase.Api.Services.Interfaces;

public interface ICustomerBookingService
{
    Task<BookingConfirmationDto?> CreateBookingAsync(
        int customerId,
        CreateBookingDto dto);

    Task<BookingConfirmationDto?> GetBookingConfirmationAsync(
        int bookingId,
        int customerId);

    Task<List<MyBookingDto>> GetMyBookingsAsync(int customerId);

    Task<bool> CancelBookingAsync(int bookingId, int customerId);
}
