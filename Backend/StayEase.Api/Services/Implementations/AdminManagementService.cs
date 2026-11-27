using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using StayEase.Api.DTOs.Admin;
using StayEase.Api.Enums;
using StayEase.Api.Models;
using StayEase.Api.Repositories;
using StayEase.Api.Services.Interfaces;

namespace StayEase.Api.Services.Implementations;

public class AdminManagementService : IAdminManagementService
{
    private readonly IUserRepository _users;
    private readonly IHotelRepository _hotels;
    private readonly IBookingRepository _bookings;
    private readonly IFacilityRepository _facilities;
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AdminManagementService(IUserRepository users, IHotelRepository hotels, IBookingRepository bookings, IFacilityRepository facilities, ApplicationDbContext context)
    { _users = users; _hotels = hotels; _bookings = bookings; _facilities = facilities; _context = context; }

    public async Task<List<UserSummaryDto>> GetUsersAsync(UserRole role, string? search, CancellationToken ct = default) =>
        (await _users.GetByRoleAsync(role, search, ct)).Select(ToDto).ToList();
    public async Task<UserSummaryDto?> GetUserAsync(int id, UserRole role, CancellationToken ct = default)
    { var user = await _users.GetByIdAsync(id, ct); return user?.Role == role ? ToDto(user) : null; }
    public async Task<bool> SetUserStatusAsync(int id, UserRole role, bool active, CancellationToken ct = default)
    { var user = await _users.GetByIdAsync(id, ct); if (user is null || user.Role != role) return false; user.IsActive = active; _users.Update(user); await _context.SaveChangesAsync(ct); return true; }

    public async Task<List<HotelSummaryDto>> GetHotelsAsync(string? search, HotelStatus? status, CancellationToken ct = default) =>
        (await _hotels.SearchAsync(search, status, ct)).Select(h => new HotelSummaryDto(h.HotelId, h.HotelName, h.City, h.Country, h.Manager.FullName, h.Status, h.Rating, h.Rooms.Count)).ToList();
    public async Task<bool> SetHotelStatusAsync(int id, HotelStatus status, CancellationToken ct = default)
    { var hotel = await _context.Hotels.FindAsync([id], ct); if (hotel is null) return false; hotel.Status = status; await _context.SaveChangesAsync(ct); return true; }

    public async Task<List<BookingSummaryDto>> GetBookingsAsync(string? search, BookingStatus? status, DateTime? from, DateTime? to, CancellationToken ct = default) =>
        (await _bookings.SearchAsync(search, status, from, to, ct)).Select(b => new BookingSummaryDto(b.BookingId, b.Customer.FullName, b.Hotel.HotelName, b.Room.RoomNumber, b.CheckInDate, b.CheckOutDate, b.TotalAmount, b.Status)).ToList();
    public async Task<bool> SetBookingStatusAsync(int id, BookingStatus status, CancellationToken ct = default)
    { var booking = await _bookings.GetByIdAsync(id, ct); if (booking is null) return false; booking.Status = status; _bookings.Update(booking); await _context.SaveChangesAsync(ct); return true; }

    public async Task<List<FacilityDto>> GetFacilitiesAsync(CancellationToken ct = default) => (await _facilities.ListAsync(ct)).Select(f => new FacilityDto(f.FacilityId, f.FacilityName, f.Description)).ToList();
    public async Task<FacilityDto?> CreateFacilityAsync(CreateFacilityDto dto, CancellationToken ct = default)
    { var name = dto.FacilityName.Trim(); if (await _facilities.NameExistsAsync(name, null, ct)) return null; var f = new Facility { FacilityName = name, Description = dto.Description?.Trim() }; await _facilities.AddAsync(f, ct); await _context.SaveChangesAsync(ct); return new(f.FacilityId, f.FacilityName, f.Description); }
    public async Task<FacilityDto?> UpdateFacilityAsync(int id, CreateFacilityDto dto, CancellationToken ct = default)
    { var f = await _facilities.GetByIdAsync(id, ct); if (f is null || await _facilities.NameExistsAsync(dto.FacilityName.Trim(), id, ct)) return null; f.FacilityName = dto.FacilityName.Trim(); f.Description = dto.Description?.Trim(); _facilities.Update(f); await _context.SaveChangesAsync(ct); return new(f.FacilityId, f.FacilityName, f.Description); }
    public async Task<bool> DeleteFacilityAsync(int id, CancellationToken ct = default)
    { var f = await _facilities.GetByIdAsync(id, ct); if (f is null) return false; _facilities.Remove(f); await _context.SaveChangesAsync(ct); return true; }

    public async Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default) => await _context.Categories.AsNoTracking().OrderBy(c => c.CategoryName).Select(c => new CategoryDto(c.CategoryId, c.CategoryName, c.Description, c.Hotels.Count)).ToListAsync(ct);
    public async Task<CategoryDto?> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken ct = default)
    { var name = dto.CategoryName.Trim(); if (await _context.Categories.AnyAsync(c => c.CategoryName == name, ct)) return null; var c = new Category { CategoryName = name, Description = dto.Description?.Trim() }; _context.Categories.Add(c); await _context.SaveChangesAsync(ct); return new(c.CategoryId, c.CategoryName, c.Description, 0); }
    public async Task<CategoryDto?> UpdateCategoryAsync(int id, CreateCategoryDto dto, CancellationToken ct = default)
    { var c = await _context.Categories.FindAsync([id], ct); if (c is null || await _context.Categories.AnyAsync(x => x.CategoryName == dto.CategoryName.Trim() && x.CategoryId != id, ct)) return null; c.CategoryName = dto.CategoryName.Trim(); c.Description = dto.Description?.Trim(); await _context.SaveChangesAsync(ct); return new(c.CategoryId, c.CategoryName, c.Description, await _context.Hotels.CountAsync(h => h.CategoryId == id, ct)); }
    public async Task<bool> DeleteCategoryAsync(int id, CancellationToken ct = default)
    { var c = await _context.Categories.FindAsync([id], ct); if (c is null) return false; _context.Categories.Remove(c); await _context.SaveChangesAsync(ct); return true; }

    public async Task<List<ReportPointDto>> GetMonthlyReportAsync(int year, CancellationToken ct = default) => await _context.Bookings.Where(b => b.BookedAt.Year == year && b.Status == BookingStatus.CheckedOut).GroupBy(b => b.BookedAt.Month).OrderBy(g => g.Key).Select(g => new ReportPointDto(g.Key.ToString(), g.Count(), g.Sum(b => b.TotalAmount))).ToListAsync(ct);
    public async Task<List<ReportPointDto>> GetPopularHotelsAsync(CancellationToken ct = default) => await _context.Bookings.Where(b => b.Status == BookingStatus.CheckedOut).GroupBy(b => b.Hotel.HotelName).OrderByDescending(g => g.Count()).Take(10).Select(g => new ReportPointDto(g.Key, g.Count(), g.Sum(b => b.TotalAmount))).ToListAsync(ct);

    public async Task<AdminProfileDto?> GetProfileAsync(int userId, CancellationToken ct = default) { var u = await _users.GetByIdAsync(userId, ct); return u?.Role == UserRole.Admin ? new(u.UserId, u.FullName, u.Email, u.PhoneNumber, u.Role) : null; }
    public async Task<AdminProfileDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto, CancellationToken ct = default)
    { var u = await _users.GetByIdAsync(userId, ct); if (u?.Role != UserRole.Admin) return null; var email = dto.Email?.Trim().ToLowerInvariant(); if (email is not null && await _context.Users.AnyAsync(x => x.Email == email && x.UserId != userId, ct)) return null; u.FullName = dto.FullName.Trim(); u.PhoneNumber = dto.PhoneNumber; if (email is not null) u.Email = email; await _context.SaveChangesAsync(ct); return new(u.UserId, u.FullName, u.Email, u.PhoneNumber, u.Role); }
    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken ct = default)
    { var u = await _users.GetByIdAsync(userId, ct); if (u?.Role != UserRole.Admin || _passwordHasher.VerifyHashedPassword(u, u.PasswordHash, dto.CurrentPassword) == PasswordVerificationResult.Failed) return false; u.PasswordHash = _passwordHasher.HashPassword(u, dto.NewPassword); await _context.SaveChangesAsync(ct); return true; }

    private static UserSummaryDto ToDto(User u) => new(u.UserId, u.FullName, u.Email, u.PhoneNumber, u.Role, u.IsActive, u.CreatedAt);
}
