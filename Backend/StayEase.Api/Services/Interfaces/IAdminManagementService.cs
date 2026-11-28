using StayEase.Api.DTOs.Admin;
using StayEase.Api.Enums;

namespace StayEase.Api.Services.Interfaces;

public interface IAdminManagementService
{
    Task<List<UserSummaryDto>> GetUsersAsync(UserRole role, string? search, CancellationToken cancellationToken = default);
    Task<UserSummaryDto?> GetUserAsync(int id, UserRole role, CancellationToken cancellationToken = default);
    Task<bool> SetUserStatusAsync(int id, UserRole role, bool active, CancellationToken cancellationToken = default);
    Task<List<HotelSummaryDto>> GetHotelsAsync(string? search, HotelStatus? status, CancellationToken cancellationToken = default);
    Task<bool> SetHotelStatusAsync(int id, HotelStatus status, CancellationToken cancellationToken = default);
    Task<List<BookingSummaryDto>> GetBookingsAsync(string? search, BookingStatus? status, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
    Task<bool> SetBookingStatusAsync(int id, BookingStatus status, CancellationToken cancellationToken = default);
    Task<List<FacilityDto>> GetFacilitiesAsync(CancellationToken cancellationToken = default);
    Task<FacilityDto?> CreateFacilityAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default);
    Task<FacilityDto?> UpdateFacilityAsync(int id, CreateFacilityDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteFacilityAsync(int id, CancellationToken cancellationToken = default);
    Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<CategoryDto?> UpdateCategoryAsync(int id, CreateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<List<ReportPointDto>> GetMonthlyReportAsync(int year, CancellationToken cancellationToken = default);
    Task<List<ReportPointDto>> GetPopularHotelsAsync(CancellationToken cancellationToken = default);
    Task<AdminProfileDto?> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<AdminProfileDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
}
