
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEase.Api.DTOs.Customer;
using StayEase.Api.Services.Interfaces;

namespace StayEase.Api.Controllers.Customer;

[ApiController]
[Route("api/customer/bookings")]
[Authorize(Roles = "Customer")]
public class CustomerBookingsController : ControllerBase
{
    private readonly ICustomerBookingService _service;

    public CustomerBookingsController(ICustomerBookingService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        int customerId = CurrentCustomerId();

        var result = await _service.CreateBookingAsync(customerId, dto);

        if (result == null)
        {
            return BadRequest(new
            {
                message = "Room is unavailable or booking details are invalid."
            });
        }

        return CreatedAtAction(
            nameof(GetConfirmation),
            new { id = result.BookingId },
            result);
    }

    [HttpGet("{id:int}/confirmation")]
    public async Task<IActionResult> GetConfirmation(int id)
    {
        int customerId = CurrentCustomerId();

        var result = await _service.GetBookingConfirmationAsync(
            id, customerId);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Booking not found."
            });
        }

        return Ok(result);
    }

    // Commit 7: View customer's bookings
    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetMyBookings()
    {
        int customerId = CurrentCustomerId();

        var result = await _service.GetMyBookingsAsync(customerId);

        return Ok(result);
    }

    // Commit 8: Cancel customer's booking
    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        int customerId = CurrentCustomerId();

        bool cancelled = await _service.CancelBookingAsync(
            id, customerId);

        if (!cancelled)
        {
            return BadRequest(new
            {
                message = "Booking cannot be cancelled. Check the booking status and check-in date."
            });
        }

        return Ok(new
        {
            message = "Booking cancelled successfully."
        });
    }

    private int CurrentCustomerId()
    {
        return int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
