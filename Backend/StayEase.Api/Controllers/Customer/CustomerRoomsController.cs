
using Microsoft.AspNetCore.Mvc;
using StayEase.Api.DTOs.Customer;
using StayEase.Api.Services.Interfaces;

namespace StayEase.Api.Controllers.Customer;

[ApiController]
[Route("api/customer/rooms")]
public class CustomerRoomsController : ControllerBase
{
    private readonly ICustomerRoomService _service;

    public CustomerRoomsController(ICustomerRoomService service)
    {
        _service = service;
    }

    [HttpGet("availability")]
    public async Task<ActionResult<List<RoomAvailabilityDto>>> GetAvailability(
        [FromQuery] int hotelId,
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut,
        [FromQuery] int guests = 1)
    {
        if (hotelId <= 0)
        {
            return BadRequest("Invalid hotel ID.");
        }

        if (checkIn.Date < DateTime.Today)
        {
            return BadRequest("Check-in date cannot be in the past.");
        }

        if (checkOut.Date <= checkIn.Date)
        {
            return BadRequest("Check-out must be after check-in.");
        }

        if (guests < 1 || guests > 20)
        {
            return BadRequest("Guests must be between 1 and 20.");
        }

        var rooms = await _service.GetAvailableRoomsAsync(
            hotelId, checkIn, checkOut, guests);

        return Ok(rooms);
    }
}
