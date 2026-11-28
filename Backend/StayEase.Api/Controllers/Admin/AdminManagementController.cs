using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEase.Api.DTOs.Admin;
using StayEase.Api.Enums;
using StayEase.Api.Services.Interfaces;

namespace StayEase.Api.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminManagementController : ControllerBase
{
    private readonly IAdminManagementService _service;
    public AdminManagementController(IAdminManagementService service) => _service = service;

    [HttpGet("customers")]
    public Task<List<UserSummaryDto>> Customers([FromQuery] string? search, CancellationToken ct) => _service.GetUsersAsync(UserRole.Customer, search, ct);
    [HttpGet("customers/{id:int}")] public async Task<IActionResult> Customer(int id, CancellationToken ct) => (await _service.GetUserAsync(id, UserRole.Customer, ct)) is { } result ? Ok(result) : NotFound();
    [HttpPatch("customers/{id:int}/status")] public async Task<IActionResult> CustomerStatus(int id, UpdateUserStatusDto dto, CancellationToken ct) => await _service.SetUserStatusAsync(id, UserRole.Customer, dto.IsActive, ct) ? NoContent() : NotFound();

    [HttpGet("managers")] public Task<List<UserSummaryDto>> Managers([FromQuery] string? search, CancellationToken ct) => _service.GetUsersAsync(UserRole.HotelManager, search, ct);
    [HttpGet("managers/{id:int}")] public async Task<IActionResult> Manager(int id, CancellationToken ct) => (await _service.GetUserAsync(id, UserRole.HotelManager, ct)) is { } result ? Ok(result) : NotFound();
    [HttpPatch("managers/{id:int}/status")] public async Task<IActionResult> ManagerStatus(int id, UpdateUserStatusDto dto, CancellationToken ct) => await _service.SetUserStatusAsync(id, UserRole.HotelManager, dto.IsActive, ct) ? NoContent() : NotFound();

    [HttpGet("hotels")] public Task<List<HotelSummaryDto>> Hotels([FromQuery] string? search, [FromQuery] HotelStatus? status, CancellationToken ct) => _service.GetHotelsAsync(search, status, ct);
    [HttpPatch("hotels/{id:int}/status")] public async Task<IActionResult> HotelStatus(int id, [FromQuery] HotelStatus status, CancellationToken ct) => await _service.SetHotelStatusAsync(id, status, ct) ? NoContent() : NotFound();
    [HttpPost("hotels/{id:int}/approve")]
    public Task<IActionResult> ApproveHotel(int id, CancellationToken ct) =>
        SetHotel(id, StayEase.Api.Enums.HotelStatus.Approved, ct);

    [HttpPost("hotels/{id:int}/reject")]
    public Task<IActionResult> RejectHotel(int id, CancellationToken ct) =>
        SetHotel(id, StayEase.Api.Enums.HotelStatus.Rejected, ct);

    [HttpPost("hotels/{id:int}/activate")]
    public Task<IActionResult> ActivateHotel(int id, CancellationToken ct) =>
        SetHotel(id, StayEase.Api.Enums.HotelStatus.Approved, ct);

    [HttpPost("hotels/{id:int}/deactivate")]
    public Task<IActionResult> DeactivateHotel(int id, CancellationToken ct) =>
        SetHotel(id, StayEase.Api.Enums.HotelStatus.Inactive, ct);

    [HttpGet("bookings")] public Task<List<BookingSummaryDto>> Bookings([FromQuery] string? search, [FromQuery] BookingStatus? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct) => _service.GetBookingsAsync(search, status, from, to, ct);
    [HttpPatch("bookings/{id:int}/status")] public async Task<IActionResult> BookingStatus(int id, UpdateBookingStatusDto dto, CancellationToken ct) => await _service.SetBookingStatusAsync(id, dto.Status, ct) ? NoContent() : NotFound();

    [HttpGet("facilities")] public Task<List<FacilityDto>> Facilities(CancellationToken ct) => _service.GetFacilitiesAsync(ct);
    [HttpPost("facilities")] public async Task<IActionResult> CreateFacility(CreateFacilityDto dto, CancellationToken ct) => (await _service.CreateFacilityAsync(dto, ct)) is { } result ? CreatedAtAction(nameof(Facilities), result) : Conflict(new { message = "Facility name already exists." });
    [HttpPut("facilities/{id:int}")] public async Task<IActionResult> UpdateFacility(int id, CreateFacilityDto dto, CancellationToken ct) => (await _service.UpdateFacilityAsync(id, dto, ct)) is { } result ? Ok(result) : NotFound();
    [HttpDelete("facilities/{id:int}")] public async Task<IActionResult> DeleteFacility(int id, CancellationToken ct) => await _service.DeleteFacilityAsync(id, ct) ? NoContent() : NotFound();

    [HttpGet("categories")] public Task<List<CategoryDto>> Categories(CancellationToken ct) => _service.GetCategoriesAsync(ct);
    [HttpPost("categories")] public async Task<IActionResult> CreateCategory(CreateCategoryDto dto, CancellationToken ct) => (await _service.CreateCategoryAsync(dto, ct)) is { } result ? Ok(result) : Conflict(new { message = "Category name already exists." });
    [HttpPut("categories/{id:int}")] public async Task<IActionResult> UpdateCategory(int id, CreateCategoryDto dto, CancellationToken ct) => (await _service.UpdateCategoryAsync(id, dto, ct)) is { } result ? Ok(result) : NotFound();
    [HttpDelete("categories/{id:int}")] public async Task<IActionResult> DeleteCategory(int id, CancellationToken ct) => await _service.DeleteCategoryAsync(id, ct) ? NoContent() : NotFound();

    [HttpGet("reports/monthly")] public Task<List<ReportPointDto>> Monthly([FromQuery] int year, CancellationToken ct) => _service.GetMonthlyReportAsync(year == 0 ? DateTime.UtcNow.Year : year, ct);
    [HttpGet("reports/popular-hotels")] public Task<List<ReportPointDto>> PopularHotels(CancellationToken ct) => _service.GetPopularHotelsAsync(ct);

    [HttpGet("profile")] public async Task<IActionResult> Profile(CancellationToken ct) => (await _service.GetProfileAsync(CurrentUserId(), ct)) is { } result ? Ok(result) : NotFound();
    [HttpPut("profile")] public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto, CancellationToken ct) => (await _service.UpdateProfileAsync(CurrentUserId(), dto, ct)) is { } result ? Ok(result) : Conflict();
    [HttpPut("profile/password")] public async Task<IActionResult> ChangePassword(ChangePasswordDto dto, CancellationToken ct) => await _service.ChangePasswordAsync(CurrentUserId(), dto, ct) ? NoContent() : BadRequest(new { message = "Current password is invalid." });

    private async Task<IActionResult> SetHotel(int id, HotelStatus status, CancellationToken ct) => await _service.SetHotelStatusAsync(id, status, ct) ? NoContent() : NotFound();
    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
