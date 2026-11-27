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

    public async Task<List<Hotel>> GetApprovedHotelsAsync()
    {
        return await _context.Hotels
            .Where(h => h.IsApproved && h.IsActive)
            .ToListAsync();
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