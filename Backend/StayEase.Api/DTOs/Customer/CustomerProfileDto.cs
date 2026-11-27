
using System.ComponentModel.DataAnnotations;

namespace StayEase.Api.DTOs.Customer;

public class CustomerProfileDto
{
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
}

public class UpdateCustomerProfileDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? PhoneNumber { get; set; }
}
