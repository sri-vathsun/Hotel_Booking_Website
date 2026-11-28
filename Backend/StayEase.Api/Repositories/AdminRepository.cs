using StayEase.Api.Data;
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _context;
    public AdminRepository(ApplicationDbContext context) => _context = context;
    public IQueryable<User> Users => _context.Users;
    public IQueryable<Hotel> Hotels => _context.Hotels;
    public IQueryable<Booking> Bookings => _context.Bookings;
    public IQueryable<Facility> Facilities => _context.Facilities;
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
}
