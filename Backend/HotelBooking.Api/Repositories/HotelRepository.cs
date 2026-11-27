using HotelBooking.Api.Data;
using HotelBooking.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Api.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly ApplicationDbContext _context;

    public HotelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Hotel>> GetApprovedHotelsAsync(
        string? search,
        string? city,
        int? minStarRating)
    {
        var query = _context.Hotels
            .Where(h => h.IsApproved && h.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(h =>
                h.Name.Contains(search) ||
                h.Description.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(h => h.City.Contains(city));
        }

        if (minStarRating.HasValue)
        {
            query = query.Where(h => h.StarRating >= minStarRating.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Hotel?> GetHotelByIdAsync(int hotelId)
    {
        return await _context.Hotels
            .FirstOrDefaultAsync(h =>
                h.HotelId == hotelId &&
                h.IsApproved &&
                h.IsActive);
    }
}