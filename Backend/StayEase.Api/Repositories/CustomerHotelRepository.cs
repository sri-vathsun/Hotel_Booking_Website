
using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using StayEase.Api.Enums;
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public class CustomerHotelRepository : ICustomerHotelRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerHotelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Hotel>> GetApprovedHotelsAsync(
        string? search,
        string? city,
        int? minStarRating)
    {
        var query = _context.Hotels
            .AsNoTracking()
            .Where(h => h.Status == HotelStatus.Approved);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(h =>
                h.HotelName.Contains(search) ||
                (h.Description != null && h.Description.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(h => h.City.Contains(city));
        }

        if (minStarRating.HasValue)
        {
            query = query.Where(h => h.Rating >= minStarRating.Value);
        }

        return await query
            .OrderBy(h => h.HotelName)
            .ToListAsync();
    }

    public async Task<Hotel?> GetHotelByIdAsync(int hotelId)
    {
        return await _context.Hotels
            .AsNoTracking()
            .FirstOrDefaultAsync(h =>
                h.HotelId == hotelId &&
                h.Status == HotelStatus.Approved);
    }
}
