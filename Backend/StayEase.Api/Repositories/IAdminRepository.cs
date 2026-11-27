using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public interface IAdminRepository
{
    IQueryable<User> Users { get; }
    IQueryable<Hotel> Hotels { get; }
    IQueryable<Booking> Bookings { get; }
    IQueryable<Facility> Facilities { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
