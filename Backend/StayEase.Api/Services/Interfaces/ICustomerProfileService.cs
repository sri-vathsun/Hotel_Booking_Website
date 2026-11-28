
using StayEase.Api.DTOs.Customer;

namespace StayEase.Api.Services.Interfaces;

public interface ICustomerProfileService
{
    Task<CustomerProfileDto?> GetProfileAsync(int customerId);

    Task<CustomerProfileDto?> UpdateProfileAsync(
        int customerId,
        UpdateCustomerProfileDto dto);
}
