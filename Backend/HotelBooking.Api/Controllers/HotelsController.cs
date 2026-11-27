using HotelBooking.Api.DTOs;
using HotelBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _service;

    public HotelsController(IHotelService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<HotelDto>>> GetHotels()
    {
        var hotels = await _service.GetApprovedHotelsAsync();

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