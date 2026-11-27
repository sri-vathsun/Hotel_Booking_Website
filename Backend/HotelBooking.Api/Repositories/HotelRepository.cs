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

    public async Task<List<Hotel>> GetApprovedHotelsAsync(string? search)
    {
        var query = _context.Hotels
            .Where(h => h.IsApproved && h.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(h =>
                h.Name.Contains(search) ||
                h.City.Contains(search) ||
                h.Address.Contains(search));
        }

        return await query.ToListAsync();
    }
}