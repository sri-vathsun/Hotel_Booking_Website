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
    public async Task<IActionResult> Customers([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) => Ok(Page(await _service.GetUsersAsync(UserRole.Customer, search, ct), page, pageSize));
    [HttpGet("customers/{id:int}")] public async Task<IActionResult> Customer(int id, CancellationToken ct) => (await _service.GetUserAsync(id, UserRole.Customer, ct)) is { } result ? Ok(result) : NotFound();
    [HttpPatch("customers/{id:int}/status")] public async Task<IActionResult> CustomerStatus(int id, UpdateUserStatusDto dto, CancellationToken ct) => await _service.SetUserStatusAsync(id, UserRole.Customer, dto.IsActive, ct) ? NoContent() : NotFound();

    [HttpGet("managers")] public async Task<IActionResult> Managers([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) => Ok(Page(await _service.GetUsersAsync(UserRole.HotelManager, search, ct), page, pageSize));
    [HttpGet("managers/{id:int}")] public async Task<IActionResult> Manager(int id, CancellationToken ct) => (await _service.GetUserAsync(id, UserRole.HotelManager, ct)) is { } result ? Ok(result) : NotFound();
    [HttpPatch("managers/{id:int}/status")] public async Task<IActionResult> ManagerStatus(int id, UpdateUserStatusDto dto, CancellationToken ct) => await _service.SetUserStatusAsync(id, UserRole.HotelManager, dto.IsActive, ct) ? NoContent() : NotFound();

    [HttpGet("hotels")] public async Task<IActionResult> Hotels([FromQuery] string? search, [FromQuery] HotelStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) => Ok(Page(await _service.GetHotelsAsync(search, status, ct), page, pageSize));
    [HttpPatch("hotels/{id:int}/status")] public async Task<IActionResult> UpdateHotelStatus(int id, [FromQuery] HotelStatus status, CancellationToken ct) => await _service.SetHotelStatusAsync(id, status, ct) ? NoContent() : NotFound();
    [HttpGet("hotels/{id:int}")] public async Task<IActionResult> Hotel(int id, CancellationToken ct) => (await _service.GetHotelAsync(id, ct)) is { } result ? Ok(result) : NotFound();
    [HttpPost("hotels/{id:int}/approve")] public Task<IActionResult> ApproveHotel(int id, CancellationToken ct) => SetHotel(id, HotelStatus.Approved, ct);
    [HttpPost("hotels/{id:int}/reject")] public Task<IActionResult> RejectHotel(int id, CancellationToken ct) => SetHotel(id, HotelStatus.Rejected, ct);
    [HttpPost("hotels/{id:int}/activate")] public Task<IActionResult> ActivateHotel(int id, CancellationToken ct) => SetHotel(id, HotelStatus.Approved, ct);
    [HttpPost("hotels/{id:int}/deactivate")] public Task<IActionResult> DeactivateHotel(int id, CancellationToken ct) => SetHotel(id, HotelStatus.Inactive, ct);

    [HttpGet("bookings")] public async Task<IActionResult> Bookings([FromQuery] string? search, [FromQuery] BookingStatus? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) => Ok(Page(await _service.GetBookingsAsync(search, status, from, to, ct), page, pageSize));
    [HttpPatch("bookings/{id:int}/status")] public async Task<IActionResult> BookingStatus(int id, UpdateBookingStatusDto dto, CancellationToken ct) => await _service.SetBookingStatusAsync(id, dto.Status, ct) ? NoContent() : NotFound();
    [HttpGet("bookings/{id:int}")] public async Task<IActionResult> Booking(int id, CancellationToken ct) => (await _service.GetBookingAsync(id, ct)) is { } result ? Ok(result) : NotFound();

    [HttpGet("facilities")] public async Task<IActionResult> Facilities([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) => Ok(Page(await _service.GetFacilitiesAsync(ct), page, pageSize));
    [HttpPost("facilities")] public async Task<IActionResult> CreateFacility(CreateFacilityDto dto, CancellationToken ct) => (await _service.CreateFacilityAsync(dto, ct)) is { } result ? CreatedAtAction(nameof(Facilities), result) : Conflict(new { message = "Facility name already exists." });
    [HttpPut("facilities/{id:int}")] public async Task<IActionResult> UpdateFacility(int id, CreateFacilityDto dto, CancellationToken ct) => (await _service.UpdateFacilityAsync(id, dto, ct)) is { } result ? Ok(result) : NotFound();
    [HttpDelete("facilities/{id:int}")] public async Task<IActionResult> DeleteFacility(int id, CancellationToken ct) => await _service.DeleteFacilityAsync(id, ct) ? NoContent() : NotFound();

    [HttpGet("categories")] public async Task<IActionResult> Categories([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) => Ok(Page(await _service.GetCategoriesAsync(ct), page, pageSize));
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
    private static PagedResult<T> Page<T>(IReadOnlyList<T> items, int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        return new PagedResult<T>(items.Skip((page - 1) * pageSize).Take(pageSize).ToList(), page, pageSize, items.Count);
    }
}
