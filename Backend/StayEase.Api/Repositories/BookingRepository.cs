using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using StayEase.Api.Enums;
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context) { }

    public Task<List<Booking>> SearchAsync(string? search, BookingStatus? status, DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var query = Set.AsNoTracking().Include(b => b.Customer).Include(b => b.Hotel).Include(b => b.Room).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b => b.Customer.FullName.Contains(search) || b.Customer.Email.Contains(search) || b.Hotel.HotelName.Contains(search));
        if (status.HasValue) query = query.Where(b => b.Status == status.Value);
        if (from.HasValue) query = query.Where(b => b.CheckInDate >= from.Value);
        if (to.HasValue) query = query.Where(b => b.CheckOutDate <= to.Value);
        return query.OrderByDescending(b => b.BookedAt).ToListAsync(cancellationToken);
    }
}
