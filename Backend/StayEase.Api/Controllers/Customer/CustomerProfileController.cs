
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEase.Api.DTOs.Customer;
using StayEase.Api.Services.Interfaces;

namespace StayEase.Api.Controllers.Customer;

[ApiController]
[Route("api/customer/profile")]
[Authorize(Roles = "Customer")]
public class CustomerProfileController : ControllerBase
{
    private readonly ICustomerProfileService _service;

    public CustomerProfileController(ICustomerProfileService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        int customerId = CurrentCustomerId();

        var result = await _service.GetProfileAsync(customerId);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Customer profile not found."
            });
        }

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(
        UpdateCustomerProfileDto dto)
    {
        int customerId = CurrentCustomerId();

        var result = await _service.UpdateProfileAsync(customerId, dto);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Customer profile not found."
            });
        }

        return Ok(result);
    }

    private int CurrentCustomerId()
    {
        return int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
