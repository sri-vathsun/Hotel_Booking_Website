
using Microsoft.AspNetCore.Mvc;
using StayEase.Api.DTOs.Customer;
using StayEase.Api.Services.Interfaces;

namespace StayEase.Api.Controllers.Customer;

[ApiController]
[Route("api/Hotels")]
public class CustomerHotelsController : ControllerBase
{
    private readonly ICustomerHotelService _service;

    public CustomerHotelsController(ICustomerHotelService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<HotelDto>>> GetHotels(
        [FromQuery] string? search,
        [FromQuery] string? city,
        [FromQuery] int? minStarRating)
    {
        var hotels = await _service.GetApprovedHotelsAsync(
            search, city, minStarRating);

        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDto>> GetHotelById(int id)
    {
        var hotel = await _service.GetHotelByIdAsync(id);

        if (hotel == null)
        {
            return NotFound(new
            {
                message = "Hotel not found."
            });
        }

        return Ok(hotel);
    }
}
