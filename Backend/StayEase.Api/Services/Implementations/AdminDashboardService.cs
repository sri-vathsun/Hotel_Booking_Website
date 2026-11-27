using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using StayEase.Api.DTOs.Admin;
using StayEase.Api.Enums;
using StayEase.Api.Services.Interfaces;

namespace StayEase.Api.Services.Implementations
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var stats = new DashboardStatsDto
            {
                TotalCustomers = await _context.Users
                    .CountAsync(u => u.Role == UserRole.Customer),

                TotalHotelManagers = await _context.Users
                    .CountAsync(u => u.Role == UserRole.HotelManager),

                TotalHotels = await _context.Hotels.CountAsync(),

                PendingApprovals = await _context.Hotels
                    .CountAsync(h => h.Status == HotelStatus.Pending),

                TotalRooms = await _context.Rooms.CountAsync(),

                TotalBookings = await _context.Bookings.CountAsync(),

                PendingBookings = await _context.Bookings
                    .CountAsync(b => b.Status == BookingStatus.Pending)
            };

            return stats;
        }
    }
}