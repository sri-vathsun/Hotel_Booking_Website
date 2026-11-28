using System.ComponentModel.DataAnnotations;
using StayEase.Api.Enums;

namespace StayEase.Api.DTOs.Admin;

public record UserSummaryDto(int UserId, string FullName, string Email, string? PhoneNumber, UserRole Role, bool IsActive, DateTime CreatedAt);
public record HotelSummaryDto(int HotelId, string HotelName, string City, string Country, string ManagerName, HotelStatus Status, decimal Rating, int RoomCount);
public record BookingSummaryDto(int BookingId, string CustomerName, string HotelName, string RoomNumber, DateTime CheckInDate, DateTime CheckOutDate, decimal TotalAmount, BookingStatus Status);
public record FacilityDto(int FacilityId, string FacilityName, string? Description);
public record CategoryDto(int CategoryId, string CategoryName, string? Description, int HotelCount);
public record ReportPointDto(string Label, int Bookings, decimal Revenue);
public record AdminProfileDto(int UserId, string FullName, string Email, string? PhoneNumber, UserRole Role);

public class UpdateUserStatusDto { public bool IsActive { get; set; } }
public class UpdateBookingStatusDto { [Required] public BookingStatus Status { get; set; } }
public class CreateFacilityDto { [Required, StringLength(100)] public string FacilityName { get; set; } = string.Empty; [StringLength(250)] public string? Description { get; set; } }
public class CreateCategoryDto { [Required, StringLength(100)] public string CategoryName { get; set; } = string.Empty; [StringLength(250)] public string? Description { get; set; } }
public class UpdateProfileDto { [Required, StringLength(100)] public string FullName { get; set; } = string.Empty; [Phone, StringLength(15)] public string? PhoneNumber { get; set; } [EmailAddress] public string? Email { get; set; } }
public class ChangePasswordDto { [Required] public string CurrentPassword { get; set; } = string.Empty; [Required, MinLength(8)] public string NewPassword { get; set; } = string.Empty; }
