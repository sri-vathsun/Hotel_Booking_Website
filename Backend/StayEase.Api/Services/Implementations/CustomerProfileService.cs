
using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using StayEase.Api.DTOs.Customer;
using StayEase.Api.Enums;
using StayEase.Api.Services.Interfaces;

namespace StayEase.Api.Services.Implementations;

public class CustomerProfileService : ICustomerProfileService
{
    private readonly ApplicationDbContext _context;

    public CustomerProfileService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerProfileDto?> GetProfileAsync(int customerId)
    {
        var customer = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                u.UserId == customerId &&
                u.Role == UserRole.Customer);

        if (customer == null)
        {
            return null;
        }

        return new CustomerProfileDto
        {
            UserId = customer.UserId,
            FullName = customer.FullName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber
        };
    }

    public async Task<CustomerProfileDto?> UpdateProfileAsync(
        int customerId,
        UpdateCustomerProfileDto dto)
    {
        var customer = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.UserId == customerId &&
                u.Role == UserRole.Customer);

        if (customer == null)
        {
            return null;
        }

        customer.FullName = dto.FullName.Trim();
        customer.PhoneNumber = dto.PhoneNumber?.Trim();

        await _context.SaveChangesAsync();

        return new CustomerProfileDto
        {
            UserId = customer.UserId,
            FullName = customer.FullName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber
        };
    }
}
