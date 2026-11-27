using StayEase.Api.Enums;
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<List<Booking>> SearchAsync(string? search, BookingStatus? status, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
}
